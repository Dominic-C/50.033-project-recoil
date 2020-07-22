﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    // Start is called before the first frame update
    private Material matWhite;
    private Material matDefault;
    private UnityEngine.Object explosionRef;
    private bool isHit;

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
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = Color.white;
        patrol();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile") && !isHit)
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            health--; // 1 damage
            isHit = true;
            spriteRenderer.material = matWhite;
            Debug.Log("remaining HP: " + health);
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
