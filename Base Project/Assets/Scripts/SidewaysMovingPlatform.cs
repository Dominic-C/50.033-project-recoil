using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewaysMovingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public float startWaitTime;
    public Transform[] moveSpots;
    protected float waitTime;
    private int position;
    public float moveSpeed;

    void Start()
    {
        position = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(moveSpots[position].position.x, transform.position.y), moveSpeed * Time.deltaTime);

        // check if it has reached the spot (with some error margin), if yes, then move to another random location
        if (Vector2.Distance(transform.position, new Vector2(moveSpots[position].position.x, transform.position.y)) < 0.02f)
        {
            if (waitTime <= 0)
            {
                position = (position + 1) % moveSpots.Length; // move to a new random location
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }




}
