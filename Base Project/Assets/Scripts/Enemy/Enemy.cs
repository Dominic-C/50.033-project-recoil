using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Transform player;
    public Transform eyes;
    public float moveSpeed;
    public float chaseSpeed;
    public float rangeOfSight;
    public AnimationClip idleAnimationClip;
    public AnimationClip runAnimationClip;
    public int health;

    // initialize protected variables in child classes
    protected Animator animator;
    protected Rigidbody2D rb2d;
    protected SpriteRenderer spriteRenderer;

    // variables for patrolling
    public float startWaitTime;
    public Transform[] moveSpots;
    protected bool isFacingLeft;
    protected int randomSpot;
    protected float waitTime;

    // variables for hitbox
    protected GameObject hitbox;


    // Start is called before the first frame update
    void Start()
    {
        // base class start function will not be inherited by children classes
    }

    // Update is called once per frame
    void Update()
    {
        // base class update function should be empty
    }

    // function to tell if the enemy is on the left or right relative to some target point
    public bool isLeft(float CurrentPoint, float TargetPoint)
    {
        if (CurrentPoint < TargetPoint)
        {
            return true;
        }
        return false;
    }
    public bool isRight(float CurrentPoint, float TargetPoint)
    {
        if (CurrentPoint > TargetPoint)
        {
            return true;
        }
        return false;
    }

    // can also add IsAbove or IsBelow if required, but the idea is the same

    // line of sight function
    public bool canSeePlayer(float distance)
    {
        bool val = false;
        float castDist = distance;

        if (isFacingLeft)
        {
            castDist = -distance;
        }
        
        Vector2 endPos = eyes.position + Vector3.right * castDist;

        // ray intercepts with things with the layer tag "Action" only. goes through all other objects.
        RaycastHit2D hit = Physics2D.Linecast(eyes.position, endPos, 1 << LayerMask.NameToLayer("Action"));

        if (hit.collider != null)
        {
            // check if the ray hits the player
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                val = true;
            }
            else
            {
                val = false;
            }
        }

        // visual guide
        if (!val)
        {
            // cannot see player
            Debug.DrawLine(eyes.position, endPos, Color.blue);
        }
        else
        {
            // can see player
            Debug.DrawLine(eyes.position, endPos, Color.yellow);
        }
        return val;

    }

    // function to patrol randomly between a set of waypoints. To be called in update function of child class
    protected void patrol()
    {
        // ====================================== Patrol Logic ========================================
        // move towards x position only, ignoring y positions
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(moveSpots[randomSpot].position.x, transform.position.y), moveSpeed * Time.deltaTime);

        // check if it has reached the spot (with some error margin), if yes, then move to another random location
        if (Vector2.Distance(transform.position, new Vector2(moveSpots[randomSpot].position.x, transform.position.y)) < 0.02f)
        {
            if (waitTime <= 0)
            {
                randomSpot = UnityEngine.Random.Range(0, moveSpots.Length); // move to a new random location
                waitTime = startWaitTime;
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

    // function to chase the player. To be called in update function of child class.
    protected void chasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            animator.Play(runAnimationClip.name);
            spriteRenderer.flipX = false;
            isFacingLeft = false;
            rb2d.velocity = new Vector2(chaseSpeed, 0);
        }
        else
        {
            animator.Play(runAnimationClip.name);
            spriteRenderer.flipX = true;
            isFacingLeft = true;
            rb2d.velocity = new Vector2(-chaseSpeed, 0);
        }

    }
}
