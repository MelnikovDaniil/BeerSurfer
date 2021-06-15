using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public event Action OnDeathEvent;
    public float jumpForce;
    public float sensitive = 0.3f;

    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private Vector2? firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;


    private bool isDead = false;
    private bool isGrounded = true;
    private bool isFalling = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
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
                if (currentSwipe.y < 0 && currentSwipe.x > -sensitive && currentSwipe.x < sensitive)
                {
                    firstPressPos = null;
                    Slip();
                }
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

    private void Death()
    {
        _animator.Play("death");
        isDead = true;
        OnDeathEvent?.Invoke();
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
        var obstacle = collision.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            Death();
        }

        if (collision.tag == "Beer")
        {
            Destroy(collision.gameObject);
            GameManager.beer++;
        }

    }
}
