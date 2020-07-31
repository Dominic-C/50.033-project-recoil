using UnityEngine;

public class BasicEnemy : Enemy
{
    // Start is called before the first frame update
    private Material matWhite;
    private Material matDefault;
    private UnityEngine.Object explosionRef;
    private bool isHit;
    private AudioSource deathSound;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        waitTime = startWaitTime;
        animator = GetComponent<Animator>();
        randomSpot = UnityEngine.Random.Range(0, moveSpots.Length);
        spriteRenderer = GetComponent<SpriteRenderer>();
        isFacingLeft = false;
        health = 1;
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material; // cast as material. By default, Resources.Load returns Object
        matDefault = spriteRenderer.material;
        explosionRef = Resources.Load("Explosion");
        isHit = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        deathSound = GetComponent<AudioSource>();
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
        if ((collision.CompareTag("PlayerProjectile") || collision.CompareTag("PlayerProjectileRocket")) && !isHit)
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            health--; // 1 damage
            isHit = true;
            spriteRenderer.material = matWhite;
            if (health <= 0)
            {
                KillSelf();
            }
            else
            {
                Invoke("ResetMaterial", 0.1f);
            }
        }
    }

    private void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
    }

    private void KillSelf()
    {
        AudioSource.PlayClipAtPoint(deathSound.clip, transform.position);
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        Destroy(explosion, 5.0f);
        Destroy(gameObject);
    }
}
