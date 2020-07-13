using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed;
    private Transform player;
    private bool lockedOn;
    // Start is called before the first frame update
    public float radius;
    public int lifetime;
    private Animator animator;
    void Start()
    {
        lockedOn = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gameObject.GetComponent<CircleCollider2D>().radius = radius;
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lockedOn)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            animator.Play("HomingProjectile_activated");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.Play("HomingProjectile_lockedon");
            lockedOn = true;
            Destroy(gameObject, lifetime);
        }
    }
}
