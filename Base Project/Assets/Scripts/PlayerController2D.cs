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
    public static bool isFacingRight, prevFaceRight;
    public static bool isGrounded;
    public static bool isJustGrounded;
    private bool leftPressed;
    private bool rightPressed;
    private bool hitWall;

    [SerializeField]
    Transform groundCheck1;

    [SerializeField]
    Transform groundCheck2;

    [SerializeField]
    Transform groundCheck3;

    [SerializeField]
    Transform wallCheck1;

    [SerializeField]
    Transform wallCheck2;
    
    public float runSpeed;
    public float jumpSpeed;

    // UI for shotgun and rocket count. 
    private GameObject shotgunCount;
    private GameObject rocketCount;

    public AnimationClip idleAnimationClip;
    public AnimationClip runAnimationClip;
    public AnimationClip shootBottomAnimationClip;
    public AnimationClip shootFrontAnimationClip;
    public AnimationClip shootBackAnimationClip;
    public float airControlImpulse;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        isFacingRight = true;
        prevFaceRight = true;
    }

    void Update()
    {
        // Controller Mode
        // Check For Pause State
        if (!LevelManager.GameIsPaused)
        {
            // Get Inputs;
            rightPressed = Input.GetKey("d");
            leftPressed = Input.GetKey("a");

            // Reload Loop
            if (isGrounded && !isJustGrounded)
            {
                if (WeaponController.isEnabled)
                {
                    WeaponController.onGroundReload();
                }
            }
            // Check for Grounded State
            if (isGrounded)
            {
                // Move Right on ground
                if (rightPressed)
                {
                    animator.Play(runAnimationClip.name);
                    if (rb2d.velocity.x < runSpeed)
                    {
                        rb2d.AddForce(new Vector2(0.2f, 0.0f), ForceMode2D.Impulse);
                    }
                    isFacingRight = true;
                }
                // Move Left on ground
                else if (leftPressed)
                {
                    animator.Play(runAnimationClip.name);
                    if (rb2d.velocity.x > -runSpeed)
                    {
                        rb2d.AddForce(new Vector2(-0.2f, 0.0f), ForceMode2D.Impulse);
                    }
                    isFacingRight = false;
                }
                else // Idle Animation
                {
                    animator.Play(idleAnimationClip.name);
                }
            }
            else // Not Grounded
            {
                animationLogicNotGrounded();

                // Press Right in Air
                if (rightPressed)
                {
                    if (rb2d.velocity.x < runSpeed && !hitWall)
                    { rb2d.AddForce(new Vector2(airControlImpulse, 0.0f), ForceMode2D.Impulse); }
                    isFacingRight = true;
                }
                // Left Press in Air
                else if (leftPressed)
                {
                    if (rb2d.velocity.x > -runSpeed && !hitWall)
                    { rb2d.AddForce(new Vector2(-airControlImpulse, 0.0f), ForceMode2D.Impulse); }
                    isFacingRight = false;
                }
            }
            // Rotate Direction of GameObject Sprite
            // IFF we change directions now
            if (isFacingRight != prevFaceRight)
            {
                transform.Rotate(0f, 180f, 0f);
            }

            // state machine to retain state of previous frame
            isJustGrounded = isGrounded;
            prevFaceRight = isFacingRight;
        }
    }

    private void FixedUpdate()
    {
        // Setup Physics State
        if (Physics2D.Linecast(transform.position, groundCheck1.position, 1 << LayerMask.NameToLayer("Ground"))
            || Physics2D.Linecast(transform.position, groundCheck2.position, 1 << LayerMask.NameToLayer("Ground"))
            || Physics2D.Linecast(transform.position, groundCheck3.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // right wall check
        if (Physics2D.Linecast(transform.position, wallCheck1.position, 1 << LayerMask.NameToLayer("Ground"))
            || Physics2D.Linecast(transform.position, wallCheck2.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            hitWall = true;
        }
        else
        {
            hitWall = false;
        }

    }

    private void animationLogicNotGrounded()
    {
        if (isFacingRight && rb2d.velocity.x > 0)
        {
            animator.Play(shootBackAnimationClip.name);
        }

        else if (isFacingRight && rb2d.velocity.x < 0)
        {
            animator.Play(shootFrontAnimationClip.name);
        }

        else if (!isFacingRight && rb2d.velocity.x > 0)
        {
            animator.Play(shootFrontAnimationClip.name);
        }

        else if (!isFacingRight && rb2d.velocity.x < 0)
        {
            animator.Play(shootBackAnimationClip.name);
        }
        else
        {
            animator.Play(shootBottomAnimationClip.name);
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
