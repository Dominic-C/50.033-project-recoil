using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapEnemy : MonoBehaviour // dont need to inherit enemy class
{
    // Start is called before the first frame update
    private Vector2 initialPosition;
    public float activationDistance;
    private Vector2 destination;
    public Transform groundCheck;
    public float moveSpeed;
    private bool isGrounded;
    private bool trapTriggered = false;
    private bool playerTriggered;
    private Animator animator;
    public AnimationClip idleAnimation;
    public AnimationClip attackAnimation;
    private AudioSource attackSound;

    void Start()
    {
        initialPosition = gameObject.transform.position;
        animator = GetComponent<Animator>();
        // animator.Play(untriggeredAnimation.name);
    }

    // Update is called once per frame
    void Update()
    {
        playerTriggered = playerTrigger(activationDistance);
        if (playerTriggered)
        {
            activate();
        }
        else
        {
            resetTrap();
            animator.Play(idleAnimation.name);

        }
    }

    private void resetTrap()
    {
        transform.position = Vector2.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void activate()
    {
        // move until hit floor, then move back to starting position
        // while not collided, move downwards
        if (!isGrounded)
        {
            transform.position += moveSpeed * Vector3.down * Time.deltaTime;
            animator.Play(attackAnimation.name);
            attackSound.Play();

        }
        else
        {
            trapTriggered = false;
        }
    }

    private bool playerTrigger(float castDistance)
    {
        // do linecast downwards
        Vector2 endPos = transform.position + Vector3.up * -1.0f * castDistance;

        // do raycast
        RaycastHit2D hit = Physics2D.Linecast(transform.position, endPos, 1 << LayerMask.NameToLayer("Action"));

        if (hit.collider != null)
        {
            // check if the ray hits the player
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                trapTriggered = true;
            }
        }
        // visual guide
        if (!trapTriggered)
        {
            Debug.DrawLine(transform.position, endPos, Color.blue);
        }
        else
        {
            Debug.DrawLine(transform.position, endPos, Color.yellow);
        }
        return trapTriggered;
    }


}
