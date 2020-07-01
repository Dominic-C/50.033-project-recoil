using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class BasicEnemy : Enemy
{
    // Start is called before the first frame update
    public Transform[] moveSpots;
    private int randomSpot;
    private float waitTime;
    public float startWaitTime;
    private Animator animator;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        waitTime = startWaitTime;
        animator = GetComponent<Animator>();
        randomSpot = Random.Range(0, moveSpots.Length);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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

        // animation logic, directional checks implemented in base class
        if (IsLeft(transform.position.x, moveSpots[randomSpot].position.x))
        {
            animator.Play("ice_zombie_run"); // go right
            spriteRenderer.flipX = false;
        }

        else if (IsRight(transform.position.x, moveSpots[randomSpot].position.x))
        {
            animator.Play("ice_zombie_run"); // go left
            spriteRenderer.flipX = true;
        }
        else
        {
            animator.Play("ice_zombie_idle");
        }
    }
}
