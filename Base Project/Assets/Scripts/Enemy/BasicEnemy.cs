using System;
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
        randomSpot = UnityEngine.Random.Range(0, moveSpots.Length);
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingLeft = false;
        health = 5;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile"))
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            health--; // 1 damage
            Debug.Log("remaining HP: " + health);
            if (health <= 0)
            {
                KillSelf();
            }
        }
    }

    private void KillSelf()
    {
        // TODO: add death animation (particle burst? ghost come out?)
        Destroy(transform.parent.gameObject);
    }
}
