using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectileController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private GameObject playerPrefab;
    private Rigidbody2D playerRb2d;
    public WeaponData RocketWeaponData;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        playerPrefab = GameObject.FindGameObjectsWithTag("Player")[0];
        playerRb2d = playerPrefab.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = transform.TransformDirection(new Vector2(RocketWeaponData.projectileSpeed, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GameBorder"))
        {
            gameObject.SetActive(false);

            Vector3 recoilVector = playerPrefab.transform.position - transform.position;
            Vector3 recoilDirection = recoilVector.normalized;
            float recoilMagnitude = RocketWeaponData.recoilForce;

            Vector2 finalRecoil = new Vector2(recoilDirection.x * recoilMagnitude, recoilDirection.y * recoilMagnitude);
            playerRb2d.AddForce(finalRecoil);
            Debug.Log("finalRecoil: " + finalRecoil.ToString());
        }
    }
}
