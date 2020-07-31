using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] batteries;
    private int energyLevel;
    private SpriteRenderer spriteRenderer;
    private Material matWhite;
    private Material matDefault;
    //private AudioSource ineffectiveSound;


    void Start()
    {
        energyLevel = batteries.Length;
        spriteRenderer = GetComponent<SpriteRenderer>();
        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        matDefault = spriteRenderer.material;
        //ineffectiveSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        energyLevel = 0;
        for (int i =0; i < batteries.Length; i++)
        {
            if (!batteries[i].GetComponent<BatteryStatus>().isHit)
            {
                energyLevel += 1;
            }
        }

        if (energyLevel <= 0)
        {
            KillSelf();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile") || collision.CompareTag("PlayerProjectileRocket"))
        {
            collision.gameObject.SetActive(false); // send back to object pooler
            spriteRenderer.material = matWhite;
            Invoke("ResetMaterial", 0.1f);
            // ineffectiveSound.Play();

        }

    }

    private void ResetMaterial()
    {
        spriteRenderer.material = matDefault;
    }


    private void KillSelf()
    {
        Destroy(gameObject);
    }
}
