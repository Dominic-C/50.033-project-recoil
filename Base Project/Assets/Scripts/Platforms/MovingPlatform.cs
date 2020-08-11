using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // Waypoints, movement speed
    public Transform[] waypoints;
    public Transform target;
    public float moveSpeed, waitTime;
    private float waiter;
    private int position;
    // Start is called before the first frame update
    void Start()
    {
        position = 0;
        target = waypoints[position];
        waiter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.01f)
        {
            if (waiter <= 0)
            {
                position = (position + 1) % waypoints.Length;
                target = waypoints[position];
                waiter = waitTime;
            }
            else
            {
                waiter -= Time.deltaTime;
            }
        }
    }
}
