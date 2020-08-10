using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectileController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private GameObject playerPrefab;
    private Rigidbody2D playerRb2d;
    public WeaponData RocketWeaponData;
    public bool toUpdateTime = false;
    public float timeToDestroyBullet = 5.0f;
    public float timeElapsed = 0.0f;
    private UnityEngine.Object explosionRef;


    void onEnabled()
    {
        toUpdateTime = true;
    }

    void onDisabled()
    {
        toUpdateTime = false;
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        explosionRef = Resources.Load("Explosion");
        playerPrefab = GameObject.FindGameObjectsWithTag("Player")[0];
        playerRb2d = playerPrefab.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = transform.TransformDirection(new Vector2(RocketWeaponData.projectileSpeed, 0));
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeToDestroyBullet)
        {
            LaunchRecoil();
            timeElapsed = 0f;
        }
    }

    void LaunchRecoil()
    {
        Vector3 recoilVector = playerPrefab.transform.position - transform.position;
        Vector3 recoilDirection = recoilVector.normalized;
        float recoilMagnitude = RocketWeaponData.recoilForce;

        Vector2 finalRecoil = new Vector2(recoilDirection.x * recoilMagnitude, recoilDirection.y * recoilMagnitude);
        playerRb2d.AddForce(finalRecoil, ForceMode2D.Impulse);
        // playerRb2d.AddForce(finalRecoil);
        gameObject.SetActive(false);
        Debug.Log("finalRecoil: " + finalRecoil.ToString());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "GameBorder" || other.gameObject.tag == "Enemy")
        {
            LaunchRecoil();
            GameObject explosion = (GameObject)Instantiate(explosionRef);
            explosion.transform.position = new Vector3(transform.position.x, transform.position.y + transform.position.z);
            Destroy(explosion, 5.0f);
        }
    }
}
