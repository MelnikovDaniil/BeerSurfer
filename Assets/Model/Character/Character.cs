using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    public event Action OnDeathEvent;
    public event Action OnJumpEvent;
    public event Action<BeerView> OnBeerPickUpEvent;
    
    public float jumpForce;
    public float sensitive = 0.3f;
    public float drinkRepeatTime = 10f;
    public SpriteRenderer phoneRenderer;
    public Sprite phoneSprite;
    public GameObject discount;

    public GameObject maxSpeedParticles;
    [NonSerialized]
    public bool enableMovementActions;

    [NonSerialized]
    public Rigidbody2D rigidbody;
    [NonSerialized]
    public Animator animator;
    [NonSerialized]
    public bool isStartedRun = false;

    private Vector2? firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;


    private readonly List<TimedBuff> _buffs = new List<TimedBuff>();

    private bool isDead = false;
    private bool isGrounded = true;
    private bool isFalling = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        discount.SetActive(false);
        maxSpeedParticles.SetActive(false);
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
        if (differenceCoof >= 1 && !maxSpeedParticles.activeSelf)
        {
            maxSpeedParticles.SetActive(true);
        }
    }

    private void Update()
    {
        if (!isDead)
        {

            if (Input.GetMouseButtonDown(0) && !GameManager.isPaused && enableMovementActions)
            {
                //save began touch 2d point
                firstPressPos = Input.mousePosition;
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
                if (currentSwipe.y > 0 && currentSwipe.x > -sensitive && currentSwipe.x < sensitive)
                {
                    firstPressPos = null;
                    Jump();
                }
                //swipe right
                if (isStartedRun && currentSwipe.y < 0 && currentSwipe.x > -sensitive && currentSwipe.x < sensitive)
                {
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

        BuffUpdate();
    }

    public void AddBuff(TimedBuff buff)
    {
        if (!_buffs.Any(x => x.ToString() == buff.ToString()))
        {
            _buffs.Add(buff);
            buff.Activate();
        }
        else
        {
            var item = _buffs.FirstOrDefault(x => x.ToString() == buff.ToString());
            item.Update();
        }
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
            }
        }
    }
    
    private void Slip()
    {
        animator.Play("Slip");
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
    }

    private void Death()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Obstacle>() || collision.tag == "Road")
        {
            Death();
        }
        else if (collision.GetComponent<LootBoxItemView>())
        {
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
