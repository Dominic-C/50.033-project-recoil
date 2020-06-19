using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer; // used to flip player

    bool isGrounded;

    [SerializeField]
    Transform groundCheck;
    private Vector3 originalSpawnPosition; 

    [SerializeField]
    public float runSpeed;

    [SerializeField]
    public float jumpSpeed;

    [SerializeField]
    public float recoilForce;

    private Transform aimTransform;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpawnPosition = gameObject.transform.position;
        aimTransform = transform.Find("Weapon");

        LevelManager.Instance.SetMaxDistance(5); // test level progress UI
    }

    public Tuple<float, float> getRecoilValues(float recoilForce)
    {
        Debug.Log(aimTransform.eulerAngles);
        float x = recoilForce * Mathf.Cos(aimTransform.eulerAngles.z * Mathf.Deg2Rad);
        float y = recoilForce * Mathf.Sin(aimTransform.eulerAngles.z * Mathf.Deg2Rad);
        return Tuple.Create(x, y);
    }

    public void Update()
    {
        // test level progress UI
        LevelManager.Instance.SetLevelProgress(Math.Abs(gameObject.transform.position.x - originalSpawnPosition.x));

        if (Input.GetMouseButtonDown(0))
        {
            Tuple<float, float> recoilVals = getRecoilValues(recoilForce);
            rb2d.velocity = new Vector2(rb2d.velocity.x + recoilVals.Item1, rb2d.velocity.y + recoilVals.Item2);
            Debug.Log("x vel:" + recoilVals.Item1 + ", y vel: " + recoilVals.Item2);
            animator.Play("Player_jump");
        }
    }


    private void FixedUpdate()
    {
        // for debugging
        float x = recoilForce * Mathf.Cos(aimTransform.eulerAngles.z * Mathf.Deg2Rad);
        float y = recoilForce * Mathf.Sin(aimTransform.eulerAngles.z * Mathf.Deg2Rad);
        Debug.Log("angle: " + aimTransform.eulerAngles.z + ", x vel:" + x + ", y vel:" + y);

        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rb2d.velocity = new Vector2(runSpeed, rb2d.velocity.y);
            if (isGrounded)
            {
                animator.Play("Player_run");
            }
            spriteRenderer.flipX = false;
        }
        else if (Input.GetKey("a") || Input.GetKey("left"))
        {
            rb2d.velocity = new Vector2(-runSpeed, rb2d.velocity.y);
            if (isGrounded)
            {
                animator.Play("Player_run");
            }
            spriteRenderer.flipX = true;
        }
        else
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y);
            animator.Play("Player_idle");
        }

        if (Input.GetKey("space") && isGrounded)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            animator.Play("Player_jump");
        }

        // get angle of weapon
        // Debug.Log(aimTransform.eulerAngles);


    }
}
