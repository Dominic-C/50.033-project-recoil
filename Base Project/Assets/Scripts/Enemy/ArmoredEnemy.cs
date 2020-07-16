using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredEnemy : Enemy
{
    private Material matWhite;
    private Material matDefault;

    // Start is called before the first frame update
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
    }

    // Update is called once per frame
    void Update()
    {
        patrol();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectileRocket"))
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            health--;
            if (health <= 0)
            {
                KillSelf();
            }
        }
        else if (collision.CompareTag("PlayerProjectile"))
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            spriteRenderer.material = matWhite;
            Invoke("ResetMaterial", 0.1f);
        }

    }

    private void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
    }

    private void KillSelf()
    {
        // TODO: add death animation (particle burst? ghost come out?)
        Destroy(transform.parent.gameObject);
    }
}
