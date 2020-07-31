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
        patrol();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("PlayerProjectile") && !isHit && !shieldUp || collision.CompareTag("PlayerProjectileRocket")) && !isHit && !shieldUp)
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
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        Destroy(explosion, 5.0f);
        Destroy(gameObject);
    }
}
