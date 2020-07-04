using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public bool isFacingLeft;
    public float projectileSpeed;
    // Start is called before the first frame update
    private Rigidbody2D rb2d;
    private Transform player;
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isFacingLeft)
        {
            projectileSpeed = -projectileSpeed;
        }
        rb2d.velocity = transform.TransformDirection(new Vector2(projectileSpeed, 0));
    }
}
