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
    private UnityEngine.Object explosionRef;

    void Start()
    {
        lockedOn = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        explosionRef = Resources.Load("Explosion");
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer(radius);
        if (lockedOn)
        {
            animator.Play("HomingProjectile_lockedon");
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            animator.Play("HomingProjectile_activated");
        }
    }

    private void DetectPlayer(float radius)
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= radius)
        {
            lockedOn = true;
            Invoke("KillSelf", lifetime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillSelf();
        }
    }

    private void KillSelf()
    {
        GameObject explosion = (GameObject)Instantiate(explosionRef);
        explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
        Destroy(explosion, 5.0f);
        Destroy(gameObject);
    }
}
