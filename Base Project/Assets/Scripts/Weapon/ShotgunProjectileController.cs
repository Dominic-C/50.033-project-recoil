using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunProjectileController : MonoBehaviour
{
    public static float projectileSpeed = 10.0f;
    public static float recoilForce = 300.0f;
    public static float fireInterval = 0.3f;
    public static int ammoCount = 2;
    public static int maxAmmo = 2;
    

    private Rigidbody2D rb2d;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = transform.TransformDirection(new Vector2(projectileSpeed, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GameBorder"))
        {
            gameObject.SetActive(false);
        }
    }
}
