using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    // Start is called before the first frame update
    private Material matWhite;
    private Material matDefault;
    private UnityEngine.Object explosionRef;
    private bool isHit;
    private bool shieldUp;
    public GameObject shield;
    public float attackRadius;
    public AnimationClip attackAnimationClip;
    private GameObject playerObject;

    public GameObject fireballPrefab;
    private float projectileMovespeed;
    private float aimedProjectileMovespeed;
    private float timeBetweenShots;
    public float BurstShotsInterval;
    public float aimedShotsInterval;
    public GameObject postBattlecamera;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material; // cast as material. By default, Resources.Load returns Object
        matDefault = spriteRenderer.material;
        explosionRef = Resources.Load("Explosion");
        health = 1;
        isHit = false;
        shieldUp = true;
        animator = GetComponent<Animator>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        projectileMovespeed = 2f;
        aimedProjectileMovespeed = 3f;
        postBattlecamera.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (shield != null)
        {
            shieldUp = true;
        }
        else
        {
            shieldUp = false;
        }


        if (playerInRadius(attackRadius) && shieldUp)
        {
            burstAttack(6);
        }
        else if (playerInRadius(attackRadius) && !shieldUp)
        {
            shootAtPlayer();
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
            patrol();
        }

    }

    private void shootAtPlayer()
    {
        animator.Play(attackAnimationClip.name);

        if (timeBetweenShots <= 0)
        {
            //var proj = Instantiate(fireballPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            GameObject proj = BossObjectPooler.SharedInstance.GetPooledObject();
            if (proj != null)
            {
                proj.transform.position = transform.position;
                proj.SetActive(true);
            }
            
            Vector2 projectileMoveDirection = (playerObject.transform.position - transform.position).normalized * aimedProjectileMovespeed;
            proj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);
            timeBetweenShots = aimedShotsInterval;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

    private void burstAttack(int numProjectiles)
    {
        animator.Play(attackAnimationClip.name);

        if (timeBetweenShots <= 0)
        {
            float angleStep = 360f / numProjectiles;
            float angle = 30f;

            for (int i = 0; i < numProjectiles; i++)
            {
                float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * attackRadius;
                float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * attackRadius;

                Vector2 projectileVector = new Vector2(projectileDirXPosition, projectileDirYPosition);
                Vector2 projectileMoveDirection = (projectileVector - new Vector2(transform.position.x, transform.position.y)).normalized * projectileMovespeed;

                GameObject proj = BossObjectPooler.SharedInstance.GetPooledObject();
                if (proj != null)
                {
                    proj.transform.position = transform.position;
                    proj.SetActive(true);
                }
                //var proj = Instantiate(fireballPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                proj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);

                angle += angleStep;

            }
            timeBetweenShots = BurstShotsInterval;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

    private bool playerInRadius(float radius)
    {
        if (Vector2.Distance(transform.position, playerObject.transform.position) <= radius)
        {
            return true;
        }
        return false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("PlayerProjectile") && !isHit && !shieldUp || collision.CompareTag("PlayerProjectileRocket")) && !isHit && !shieldUp)
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            health--; // 1 damage
            isHit = true;
            // spriteRenderer.material = matWhite;
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
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        // Destroy(explosion, 5.0f);
        // Destroy(gameObject);
        // gameObject.SetActive(false);
        postBattlecamera.SetActive(true);
    }
}
