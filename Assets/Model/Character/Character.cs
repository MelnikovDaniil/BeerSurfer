using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public float jumpForce;

    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    public Text beerCounterText;

    private bool isGrounded = true;
    private bool isFalling = false;

    private int collectedBeer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = Input.mousePosition;

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe left
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                Jump();
            }
            //swipe right
            if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
            {
                Slip();
            }
        }

        if (!isFalling && _rigidbody.velocity.y < 0)
        {
            isFalling = true;
            _animator.SetTrigger("fall");
        }

    }

    private void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            isFalling = false;
            _animator.SetTrigger("jump");
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private void Slip()
    {
        _animator.SetTrigger("slip");
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
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
        if (collision.tag == "Beer")
        {
            Destroy(collision.gameObject);
            collectedBeer++;
            beerCounterText.text = collectedBeer.ToString();
        }
    }
}
