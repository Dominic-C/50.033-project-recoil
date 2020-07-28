using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryStatus : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isHit;
    private Animator animator;
    public AnimationClip batteryOkClip;
    public AnimationClip batteryDestroyedClip;
    void Start()
    {
        isHit = false;
        animator = GetComponent<Animator>();
        animator.Play(batteryOkClip.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile") && !isHit)
        {
            isHit = true;
            animator.Play(batteryDestroyedClip.name);
        }
    }
}
