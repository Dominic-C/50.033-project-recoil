using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryStatus : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isHit;
    public GameObject indicator;
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
        if ((collision.gameObject.CompareTag("PlayerProjectile") && !isHit) || (collision.gameObject.CompareTag("PlayerProjectileRocket") && !isHit))
        {
            isHit = true;
            collision.gameObject.SetActive(false);
            animator.Play(batteryDestroyedClip.name);
            indicator.SetActive(false);
        }
    }
}
