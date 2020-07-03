using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class BasicEnemy : Enemy
{
    // Start is called before the first frame update
    public Transform[] moveSpots;
    public float startWaitTime;
    SpriteRenderer spriteRenderer;
    
    private Rigidbody2D rb2d;
    private int randomSpot;
    private float waitTime;
    private Animator animator;
    
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        waitTime = startWaitTime;
        animator = GetComponent<Animator>();
        randomSpot = Random.Range(0, moveSpots.Length);
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingLeft = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSeePlayer(rangeOfSight))
        {
            spriteRenderer.color = Color.red;
            chasePlayer();
        }
        else
        {
            spriteRenderer.color = Color.white;
            
            // =============================== Patrol logic ======================================
            // move towards x position only, ignoring y positions
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(moveSpots[randomSpot].position.x, transform.position.y), moveSpeed * Time.deltaTime);

        
            // check if it has reached the spot (with some error margin), if yes, then move to another random location
            if (Vector2.Distance(transform.position, new Vector2(moveSpots[randomSpot].position.x, transform.position.y)) < 0.02f)
            {
                // wait for awhile
                if (waitTime <= 0)
                {
                    // do something
                    randomSpot = Random.Range(0, moveSpots.Length); // move to a new random location
                    waitTime = startWaitTime; // reset timer 
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }

            // ================================ Line of sight and animation ==================================
            if (isLeft(transform.position.x, moveSpots[randomSpot].position.x))
            {
                animator.Play(runAnimationClip.name);
                spriteRenderer.flipX = false;
                isFacingLeft = false;
            }

            else if (isRight(transform.position.x, moveSpots[randomSpot].position.x))
            {
                animator.Play(runAnimationClip.name);
                spriteRenderer.flipX = true;
                isFacingLeft = true;
            }
            else
            {
                animator.Play(idleAnimationClip.name);
            }
        
        }
    }

    void chasePlayer()
    {
        if(transform.position.x < player.position.x)
        {
            animator.Play(runAnimationClip.name);
            spriteRenderer.flipX = false;
            isFacingLeft = false;
            rb2d.velocity = new Vector2(moveSpeed, 0);
        }
        else
        {
            animator.Play(runAnimationClip.name);
            spriteRenderer.flipX = true;
            isFacingLeft = true;
            rb2d.velocity = new Vector2(-moveSpeed, 0);
        }
        
    }
}
