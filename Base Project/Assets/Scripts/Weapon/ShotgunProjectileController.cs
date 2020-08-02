using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunProjectileController : MonoBehaviour
{
    public WeaponData ShotgunWeaponData;
    private Rigidbody2D rb2d;
    public bool toUpdateTime = false;
    public float timeToDestroyBullet = 3.0f;
    public float timeElapsed = 0.0f;

    void onEnabled()
    {
        toUpdateTime = true;
    }

    void onDisabled()
    {
        toUpdateTime = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = transform.TransformDirection(new Vector2(ShotgunWeaponData.projectileSpeed, 0));
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeToDestroyBullet)
        {
            gameObject.SetActive(false);
            timeElapsed = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "GameBorder" || other.gameObject.tag == "Enemy")
        {
            gameObject.SetActive(false);
        }
    }
}
