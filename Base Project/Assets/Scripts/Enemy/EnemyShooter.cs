using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : Enemy
{
    // variables for shooting
    private float timeBetweenShots;
    public float startTimeBetweenShots; // seconds
    public GameObject projectile;
    private bool playerInSight;
    private bool isHit;
    private UnityEngine.Object explosionRef;
    public AnimationClip shootAnimationClip;
    private AudioSource deathSound;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingLeft = false;
        timeBetweenShots = 0;
        explosionRef = Resources.Load("Explosion");
        isHit = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        deathSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // player in range
        playerInSight = canSeePlayer(rangeOfSight);
        if (playerInSight)
        {
            // spriteRenderer.color = Color.red;
            shootProjectile();
            animator.Play(shootAnimationClip.name);
            Debug.Log("shooting animation playing");
        }
        else
        {
            // spriteRenderer.color = Color.white;
            timeBetweenShots -= Time.deltaTime; // this is to reload even though the player is out of sight
            patrol();
        }
    }

    // Helper methods
    void shootProjectile()
    {
        // timed shots
        if (timeBetweenShots <= 0)
        {
            if (canSeePlayer(rangeOfSight))
            {
                // shoot projectile
                GameObject projectileObject = Instantiate(projectile, eyes.position, Quaternion.identity); // instantiate with initial position and rotation
                // projectileObject.GetComponent<EnemyProjectile>().isFacingLeft = isFacingLeft;
                timeBetweenShots = startTimeBetweenShots;
                
            }
        }
        else
        {
            // "recharge" even if player not in range
            timeBetweenShots -= Time.deltaTime;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("PlayerProjectile") || collision.CompareTag("PlayerProjectileRocket")) && !isHit)
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            health--; // 1 damage
            isHit = true;
            Debug.Log("remaining HP: " + health);
            if (health <= 0)
            {
                KillSelf();
            }
            else
            {
                Invoke("ResetMaterial", 0.2f);
            }
        }
    }

    private void KillSelf()
    {
        AudioSource.PlayClipAtPoint(deathSound.clip, transform.position);
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        Destroy(explosion, 5.0f);
        Destroy(transform.gameObject);
    }

    private IEnumerator playShootAnim()
    {
        animator.Play(shootAnimationClip.name);
        yield return new WaitForSeconds(0.2f);
    }
}

