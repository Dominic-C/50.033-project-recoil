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
    private bool leftPressedInAir;
    private bool rightPressedInAir;

    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    public float runSpeed;

    [SerializeField]
    public float jumpSpeed;

    // UI for shotgun and rocket count. 
    private GameObject shotgunCount;
    private GameObject rocketCount;

    public AnimationClip idleAnimationClip;
    public AnimationClip runAnimationClip;
    public AnimationClip shootBottomAnimationClip;
    public AnimationClip shootFrontAnimationClip;
    public AnimationClip shootBackAnimationClip;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        isFacingRight = true;
    }

    void Update()
    {
        if (!LevelManager.GameIsPaused)
        {
            if (isGrounded && !isJustGrounded)
            {
                if (WeaponController.isEnabled)
                {
                    WeaponController.onGroundReload();
                }
            }

            if (Input.GetKey("d") && isGrounded)
            {
                if (isGrounded)
                {
                    // move
                    animator.Play(runAnimationClip.name);
                    rb2d.velocity = new Vector2(runSpeed, 0.0f);
                }
                else
                {
                    // play jump animation
                    animator.Play(shootFrontAnimationClip.name);
                }

                if (Input.GetKey("space"))
                {
                    isGrounded = false; // prevent double jumping
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f); // prevent inconsistent jumps
                    rb2d.AddForce(new Vector2(0.0f, 5.0f), ForceMode2D.Impulse);
                    animator.Play(shootFrontAnimationClip.name);
                }

                // flipping game object logic
                if (!isFacingRight)
                {
                    isFacingRight = true;
                    transform.Rotate(0f, 180f, 0f);  // rotating transform instead of flipping spriteRenderer would change the coordinate system of the child elements
                }
            }

            /*
            else if (Input.GetKey("d") && !isGrounded)
            {
                if (!rightPressedInAir)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x - 1f, rb2d.velocity.y);
                    rightPressedInAir = true;
                    leftPressedInAir = false;
                }
                
                // flipping game object logic
                if (!isFacingRight)
                {
                    isFacingRight = true;
                    transform.Rotate(0f, 180f, 0f);
                }
            }
            */

            else if (Input.GetKey("a") && isGrounded)
            {
                if (isGrounded)
                {
                    // move
                    animator.Play(runAnimationClip.name);
                    rb2d.velocity = new Vector2(-runSpeed, 0.0f);

                }
                else
                {
                    // play jump animation
                    animator.Play(shootFrontAnimationClip.name);
                }
                
                if (Input.GetKey("space"))
                {
                    isGrounded = false; // prevent double jumping
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f); // prevent inconsistent jumps
                    rb2d.AddForce(new Vector2(0.0f, 5.0f), ForceMode2D.Impulse);
                    animator.Play(shootFrontAnimationClip.name);
                }

                if (isFacingRight)
                {
                    isFacingRight = false;
                    transform.Rotate(0f, 180f, 0f);
                }
            }

            /*
            else if (Input.GetKey("a") && !isGrounded)
            {
                // if didnt already press up in air (remove if causes issue with gun recoil)
                if (!leftPressedInAir)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x + 1f, rb2d.velocity.y);
                    leftPressedInAir = true;
                    rightPressedInAir = false;
                }

                if (isFacingRight)
                {
                    isFacingRight = false;
                    transform.Rotate(0f, 180f, 0f);
                }
            }
            */
            else if (Input.GetKey("space") && isGrounded) // temporary to test removing of controls when jumping
            {
                isGrounded = false; // prevent double jumping
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0.0f); // prevent inconsistent jumps
                rb2d.AddForce(new Vector2(0.0f, 5.0f), ForceMode2D.Impulse);
                animator.Play(shootFrontAnimationClip.name);
            }
            else if (!isGrounded) // not grounded and no player input
            {
                // play jump animation
                animator.Play(shootFrontAnimationClip.name);

            }
            else if (isGrounded)
            {
                // play idle animation
                animator.Play(idleAnimationClip.name);
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
            leftPressedInAir = false;
            rightPressedInAir = false;
            
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
