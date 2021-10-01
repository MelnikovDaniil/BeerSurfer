using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public event Action OnDeathEvent;
    public event Action OnJumpEvent;
    public event Action<BeerView> OnBeerPickUpEvent;
    
    public float jumpForce;
    public float sensitive = 0.3f;
    public float drinkRepeatTime = 10f;
    public float batCharge = 10;
    public float finishingDelay = 2;
    public float stoppingTime = 2;
    public SpriteRenderer phoneRenderer;
    public Sprite phoneSprite;
    public GameObject discount;
    public Animator bitAnimator;
    public BitBuff batBuff;
    public AudioClip batCrush;
    
    public GameObject maxSpeedParticles;
    [NonSerialized]
    public bool enableMovementActions;

    [NonSerialized]
    public Rigidbody2D rigidbody;
    [NonSerialized]
    public Animator animator;
    [NonSerialized]
    public bool isStartedRun = false;
    [NonSerialized]
    public bool bitEnabled = false;

    public ParticleSystem crushParticlesPrefab;
    public ParticleSystem lootboxParticlesPrefab;

    public BatCounter batCounter;

    private Vector2? firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    private float clicked = 0;
    private float clicktime = 0;
    private float clickdelay = 0.3f;

    private readonly List<TimedBuff> _buffs = new List<TimedBuff>();

    private bool isDead = false;
    private bool gameEnded = false;
    private bool isGrounded = true;
    private bool isFalling = false;
    private float currentBatCharge;
    private Vector2 startPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        discount.SetActive(false);
        maxSpeedParticles.SetActive(false);

        startPosition = transform.position;
    }

    public void EnableDobleBeerBonus()
    {
        discount.SetActive(true);
    }

    private void Start()
    {
        DifficultyManger.OnDifficultyChange += OnDifficultyChange;
        enableMovementActions = true;
        if (Random.value > 0.5f)
        {
            phoneRenderer.sprite = phoneSprite;
        }
        else
        {
            InvokeRepeating(nameof(DrinkBeer), drinkRepeatTime, drinkRepeatTime);
        }
    }

    private void OnDifficultyChange(float differenceCoof)
    {
        animator.speed = 1f + differenceCoof / 3f;
        if (differenceCoof >= 1 && !maxSpeedParticles.activeSelf)
        {
            maxSpeedParticles.SetActive(true);
        }
    }

    private void Update()
    {
        if (!gameEnded)
        {

            if (!isDead)
            {
                if (currentBatCharge > 0)
                {
                    currentBatCharge -= Time.deltaTime;
                }

                if (Input.GetMouseButtonDown(0) && !GameManager.isPaused && enableMovementActions)
                {
                    clicked++;
                    if (clicked == 1) clicktime = Time.time;
                    //save began touch 2d point
                    firstPressPos = Input.mousePosition;
                }
                if (clicked > 1 && Time.time - clicktime < clickdelay)
                {
                    clicked = 0;
                    clicktime = 0;
                    if (isStartedRun && !bitEnabled
                        && BatBonusMapper.Get() > 0
                        && currentBatCharge <= 0
                        && GuideManager.Instance.WhaitingForOrNotGuide(GuideSteps.Bat))
                    {
                        BatBonusMapper.RemoveOne();
                        var bat = batBuff.InitializeBuff(gameObject);
                        AddBuff(bat);
                        currentBatCharge = batBuff.duration + batCharge;
                        batCounter.SetCooldown(currentBatCharge);
                        GuideManager.Instance.FinishStep();
                    }
                }
                else if (clicked > 2 || Time.time - clicktime > 1)
                {
                    clicked = 0;
                }

                if (firstPressPos != null && Input.GetMouseButton(0))
                {
                    //save ended touch 2d point
                    secondPressPos = Input.mousePosition;

                    //create vector from the two points
                    currentSwipe = new Vector2(secondPressPos.x - firstPressPos.Value.x, secondPressPos.y - firstPressPos.Value.y);

                    //normalize the 2d vector
                    currentSwipe.Normalize();

                    //swipe left
                    if (currentSwipe.y > 0 && currentSwipe.x > -sensitive
                        && currentSwipe.x < sensitive
                        && GuideManager.Instance.WhaitingForOrNotGuide(GuideSteps.Jump))
                    {
                        clicked = 0;
                        firstPressPos = null;
                        Jump();
                    }
                    //swipe right
                    if (isStartedRun && currentSwipe.y < 0
                        && currentSwipe.x > -sensitive
                        && currentSwipe.x < sensitive
                        && GuideManager.Instance.WhaitingForOrNotGuide(GuideSteps.Slip))
                    {
                        clicked = 0;
                        firstPressPos = null;
                        Slip();
                    }
                }

                if (firstPressPos != null && Input.GetMouseButtonUp(0))
                {
                    firstPressPos = null;
                }

                if (!isFalling && rigidbody.velocity.y < 0)
                {
                    isFalling = true;
                    animator.SetTrigger("fall");
                }
            }
            else
            {
                transform.position += Vector3.left * 10 * Time.deltaTime;
            }
        }

        BuffUpdate();
    }

    public void AddBuff(TimedBuff buff)
    {
        if (!_buffs.Any(x => x.ToString() == buff.ToString()))
        {
            _buffs.Add(buff);
            buff.Activate();
            UIManager.AddBuff(buff);
        }
        else
        {
            var item = _buffs.FirstOrDefault(x => x.ToString() == buff.ToString());
            item.Update();
        }
    }

    public void ActivateBit()
    {
        if (!bitEnabled)
        {
            bitEnabled = true;
            bitAnimator.SetBool("active", true);
        }
    }

    public void DeactivateBit()
    {
        bitEnabled = false;
        bitAnimator.SetBool("active", false);
    }

    public void SecondLife(float delay)
    {
        animator.speed = 1;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("SecondLife");
        isDead = false;
        transform.position = startPosition;
        Invoke(nameof(SecondLifeEnd), delay);
    }

    public void SecondLifeEnd()
    {
        animator.updateMode = AnimatorUpdateMode.Normal;
    }

    private void BuffUpdate()
    {
        var buffs = _buffs.ToArray();
        foreach (var buff in buffs)
        {
            buff.Tick(Time.deltaTime);
            if (buff.IsFinished)
            {
                _buffs.Remove(buff);
            }
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            OnJumpEvent?.Invoke();
            if (isStartedRun)
            {
                isGrounded = false;
                isFalling = false;
                animator.Play("Jump");
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                GuideManager.Instance.FinishStep();
            }
        }
    }
    
    private void Slip()
    {
        animator.Play("Slip", 0, 0);
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
        GuideManager.Instance.FinishStep();
    }

    private void Death()
    {
        _buffs.ForEach(x => x.End());
        animator.SetTrigger("death");
        isDead = true;
        OnDeathEvent?.Invoke();
    }

    private void DrinkBeer()
    {
        if (!isStartedRun)
        {
            animator.SetTrigger("beer");
        }
        else
        {
            CancelInvoke(nameof(DrinkBeer));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetBool("run", true);
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("run", false);
        isGrounded = false;
    }

    private IEnumerator Stopping()
    {
        yield return new WaitForSeconds(finishingDelay);
        LocationGenerator.Instance.StopMoving();
        animator.Play("Stopping");
        yield return new WaitForSeconds(stoppingTime);
        ShakingManager.Instance.StartShaking();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "GuideObstacle" && GuideMapper.IsActive())
        {
            GuideManager.Instance.ActivateStep();
        }
        else if (collision.GetComponent<Obstacle>())
        {
            if (bitEnabled)
            {
                var chushParitcles = Instantiate(
                    crushParticlesPrefab,
                    collision.transform.parent);
                crushParticlesPrefab.transform.localPosition = new Vector3(collision.transform.localPosition.x, 0);
                Destroy(chushParitcles.gameObject, 1f);
                Destroy(collision.gameObject);
                var bat = _buffs.First(x => x.Buff.Name == "Bat");
                SoundManager.PlaySound(batCrush);
                bat.End();
                bat.IsFinished = true;
                _buffs.Remove(bat);
            }
            else
            {
                Death();
            }
        }
        else if (collision.GetComponent<RoadPart>())
        {
            if (collision.tag == "LevelEnding")
            {
                gameEnded = true;
                StartCoroutine(Stopping());
                UIManager.Finish();
            }
            else
            {
                Death();
            }
        }
        else if (collision.GetComponent<LootBoxItemView>())
        {
            SoundManager.PlaySound("lootbox"); 
            var particles = Instantiate(lootboxParticlesPrefab, transform.position, Quaternion.identity, transform);
            particles.Play();
            Destroy(collision.gameObject);
            LootBoxMapper.AddOne();
        }
        else if (collision.TryGetComponent<BeerView>(out var beer))
        {
            OnBeerPickUpEvent?.Invoke(beer);
        }
        else if (collision.TryGetComponent<BonusView>(out var bonus))
        {
            var buff = bonus.buffInitializator.InitializeBuff(gameObject);
            AddBuff(buff);
            bonus.PickUp();
            Destroy(collision.gameObject);
        }
    }
}
