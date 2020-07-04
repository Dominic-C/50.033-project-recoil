using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public bool projectileOnLeft;
    public float projectileSpeed;
    // Start is called before the first frame update
    private Rigidbody2D rb2d;
    private Transform player;
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (transform.position.x > player.position.x)
        {
            projectileSpeed = -projectileSpeed;
        }
    }

    private void Update()
    {
        rb2d.velocity = transform.TransformDirection(new Vector2(projectileSpeed, 0));
    }
}
