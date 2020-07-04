using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class BasicEnemy : Enemy
{
    // Start is called before the first frame update

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
            patrol();
        }
    }


}
