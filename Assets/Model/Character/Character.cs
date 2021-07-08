using System;
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

    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private Vector2? firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    [NonSerialized]
    public bool isStartedRun = false;

    private bool isDead = false;
    private bool isGrounded = true;
    private bool isFalling = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (Random.value > 0.5f)
        {
            phoneRenderer.sprite = phoneSprite;
        }
        else
        {
            InvokeRepeating(nameof(DrinkBeer), drinkRepeatTime, drinkRepeatTime);
        }
    }

    private void Update()
    {
        if (!isDead)
        {

            if (Input.GetMouseButtonDown(0))
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

            if (!isFalling && _rigidbody.velocity.y < 0)
            {
                isFalling = true;
                _animator.SetTrigger("fall");
            }
        }
        else
        {
            transform.position += Vector3.left * 10 * Time.deltaTime;
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
                _animator.Play("Jump");
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
    
    private void Slip()
    {
        _animator.Play("Slip");
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
    }

    private void Death()
    {
        _animator.SetTrigger("death");
        isDead = true;
        OnDeathEvent?.Invoke();
    }

    private void DrinkBeer()
    {
        if (!isStartedRun)
        {
            _animator.SetTrigger("beer");
        }
        else
        {
            CancelInvoke(nameof(DrinkBeer));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _animator.SetBool("run", true);
        isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _animator.SetBool("run", false);
        isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Obstacle>())
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
    }
}
