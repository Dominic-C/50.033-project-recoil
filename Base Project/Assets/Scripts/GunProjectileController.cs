using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectileController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float gunProjectileSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = transform.TransformDirection(new Vector2(gunProjectileSpeed, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag ("GameBorder"))
        {
            gameObject.SetActive(false);
        }
    }
}
