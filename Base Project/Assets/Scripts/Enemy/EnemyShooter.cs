using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Enemy
{
    SpriteRenderer spriteRenderer;

    private Rigidbody2D rb2d;
    private int randomSpot;
    private float waitTime;
    private Animator animator;

    // variables for shooting
    private float timeBetweenShots;
    public float startTimeBetweenShots; // seconds
    public GameObject projectile;
    private bool playerInSight;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingLeft = false;
        timeBetweenShots = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // player in range
        playerInSight = canSeePlayer(rangeOfSight);
        if (playerInSight)
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
        // timed shots
        if (timeBetweenShots <=0)
        {
            if (playerInSight)
            {
                // shoot projectile
                GameObject projectileObject = Instantiate(projectile, eyes.position, Quaternion.identity); // instantiate with initial position and rotation
                projectileObject.GetComponent<EnemyProjectile>().isFacingLeft = isFacingLeft;
                timeBetweenShots = startTimeBetweenShots;
            }
        }
        else
        {
            // "recharge" even if player not in range
            timeBetweenShots -= Time.deltaTime;
        }
     }
}

