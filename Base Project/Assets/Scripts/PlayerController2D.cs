using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController2D : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;

    // states
    public static bool isFacingRight;
    public static bool isGrounded;
    public static bool isJustGrounded;


    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    public float runSpeed;

    [SerializeField]
    public float jumpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        isFacingRight = true;
    }

    void Update()
    {
        // only take in input when game is not paused
        if (!LevelManager.GameIsPaused)
        {
            if (isGrounded)
            {
                // jump logic
                if (Input.GetButtonDown("Jump"))
                {
                    // jumpSpeed value is originally 3
                    Vector2 newForce = new Vector2(rb2d.velocity.x, jumpSpeed);
                    rb2d.AddForce(newForce);
                    animator.Play("Player_jump");
                }
            }

            if (isGrounded && !isJustGrounded)
            {
                if (WeaponController.isEnabled)
                {
                    WeaponController.onGroundReload();
                }
            }

            // move player horizontally based on input
            float horizontalTranslate = Input.GetAxis("Horizontal");
            if (horizontalTranslate == 1)  // right button is pressed
            {
                // rb2d.AddForce(new Vector2(runSpeed, rb2d.velocity.y));
                rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
                if (isGrounded)
                {
                    animator.Play(AnimationType.PlayerRun);
                }

                // rotate player and gun based on change in direction
                if (!isFacingRight)
                {
                    isFacingRight = true;
                    transform.Rotate(0f, 180f, 0f);  // rotating transform instead of flipping spriteRenderer would change the coordinate system of the child elements
                }
            }
            else if (horizontalTranslate == -1)  // left button is pressed
            {
                rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
                if (isGrounded)
                {
                    animator.Play(AnimationType.PlayerRun);
                }

                // rotate player and gun based on change in direction
                if (isFacingRight)
                {
                    isFacingRight = false;
                    transform.Rotate(0f, 180f, 0f);
                }
            }
            else
            {
                animator.Play(AnimationType.PlayerIdle);
            }

            // state machine to retain state of previous frame
            isJustGrounded = isGrounded;
        }
    }

    private void FixedUpdate()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {

            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("NextLevelDoor"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (col.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player dies");
            LevelManager.onPlayerDeath();
        }
        else if (col.gameObject.CompareTag("EnemyProjectile"))
        {
            Debug.Log("Player dies");
            Destroy(col.gameObject);
            LevelManager.onPlayerDeath();
        }
        else if (col.gameObject.CompareTag("HomingProjectile"))
        {
            Debug.Log("Player dies");
            Destroy(col.gameObject.transform.parent.gameObject);
            LevelManager.onPlayerDeath();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NextLevelDoor"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player dies");
            LevelManager.onPlayerDeath();
        }
    }
}
