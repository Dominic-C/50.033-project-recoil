using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public Transform eyes;
    public float moveSpeed;
    public float rangeOfSight;
    public AnimationClip idleAnimationClip;
    public AnimationClip runAnimationClip;
    protected bool isFacingLeft;
    public bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isLeft(float CurrentPoint, float TargetPoint)
    {
        if (CurrentPoint < TargetPoint)
        {
            return true;
        }
        return false;
    }
    public bool isRight(float CurrentPoint, float TargetPoint)
    {
        if (CurrentPoint > TargetPoint)
        {
            return true;
        }
        return false;
    }

    // can also add IsAbove or IsBelow if required, but the idea is the same

    public bool canSeePlayer(float distance)
    {
        bool val = false;
        float castDist = distance;

        if (isFacingLeft)
        {
            castDist = -distance;
        }
        
        Vector2 endPos = eyes.position + Vector3.right * castDist;

        // ray intercepts with things with the layer tag "Action" only. goes through all other objects.
        RaycastHit2D hit = Physics2D.Linecast(eyes.position, endPos, 1 << LayerMask.NameToLayer("Action"));

        if (hit.collider != null)
        {
            // check if the ray hits the player
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                val = true;
            }
            else
            {
                val = false;
            }
        }

        if (!val)
        {
            // cannot see player
            Debug.DrawLine(eyes.position, eyes.position + Vector3.right * castDist, Color.blue);
        }
        else
        {
            // can see player
            Debug.DrawLine(eyes.position, eyes.position + Vector3.right * castDist, Color.yellow);
        }
        return val;

    }


}
