using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject FirepointPrefab;

    private GameObject shotgunCount;
    private GameObject rocketCount;
    private GameObject equippedGunText;

    private GameObject projectile;
    private GameObject projectile0;
    private GameObject projectile1;
    private GameObject projectile2;

    // private Transform aimTransform;
    private Rigidbody2D rb2d;

    private string equippedGun;
    
    private float shotgunRecoilForce;

    private float shotgunNextFireTime = 0.0f;  // in seconds
    private float rocketNextFireTime = 0.0f;

    private float onGroundReloadInterval = 3.0f;
    public static float nextReloadTime = 0.0f;

    private void Start()
    {
        rb2d = PlayerPrefab.GetComponent<Rigidbody2D>();

        shotgunCount = GameObject.Find("ShotgunCount");
        rocketCount = GameObject.Find("RocketCount");
        equippedGunText = GameObject.Find("EquippedGun");

        equippedGun = "shotgun";

        shotgunRecoilForce = ShotgunProjectileController.recoilForce;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // calc direction of aim
        Vector3 aimDirection = (mouseWorldPosition - transform.position).normalized;
        
        // apply transformation based on whether its flipped
        if (PlayerController2D.isFacingRight)
        {
            // convert to euler angles
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, angle);
        }
        else
        {
            // convert to euler angles
            float angle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
            angle = angle + 90;  // i forgot the math, not sure why need to +90 degrees in this case
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, angle);
        }

        if (Time.time > nextReloadTime)
        {
            // reset shotgun ammo
            ShotgunProjectileController.ammoCount = ShotgunProjectileController.maxAmmo;
            shotgunCount.GetComponent<UnityEngine.UI.Text>().text = "Shotgun ammo: " + ShotgunProjectileController.ammoCount.ToString() + "/" + ShotgunProjectileController.maxAmmo.ToString();

            // reset rocket ammo
            RocketProjectileController.ammoCount = RocketProjectileController.maxAmmo;
            rocketCount.GetComponent<UnityEngine.UI.Text>().text = "Rocket ammo: " + RocketProjectileController.ammoCount.ToString() + "/" + RocketProjectileController.maxAmmo.ToString();

            // ensure that ammo will only be reloaded once (esp in combination with the reload when landing on the ground)
            nextReloadTime = float.MaxValue;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (equippedGun == "shotgun")
            {
                if (ShotgunProjectileController.ammoCount > 0 && Time.time > shotgunNextFireTime)
                {
                    // Generate projectile for shotgun
                    Tuple<float, float> recoilVals = getRecoilValues(shotgunRecoilForce);
                    float horizontalForce = rb2d.velocity.x + recoilVals.Item1;
                    float verticalForce = rb2d.velocity.y + recoilVals.Item2;
                    rb2d.AddForce(new Vector2(horizontalForce, verticalForce));
                    
                    projectile0 = ObjectPooler.Instance.SpawnFromPool("shotgun");
                    if (projectile0 != null)
                    {
                        projectile0.transform.position = FirepointPrefab.transform.position;
                        projectile0.transform.eulerAngles = FirepointPrefab.transform.eulerAngles + new Vector3(0,0,5);
                        projectile0.SetActive(true);
                    }
                    projectile1 = ObjectPooler.Instance.SpawnFromPool("shotgun");
                    if (projectile1 != null)
                    {
                        projectile1.transform.position = FirepointPrefab.transform.position;
                        projectile1.transform.eulerAngles = FirepointPrefab.transform.eulerAngles;
                        projectile1.SetActive(true);
                    }
                    projectile2 = ObjectPooler.Instance.SpawnFromPool("shotgun");
                    if (projectile2 != null)
                    {
                        projectile2.transform.position = FirepointPrefab.transform.position;
                        projectile2.transform.eulerAngles = FirepointPrefab.transform.eulerAngles + new Vector3(0,0,-5);
                        projectile2.SetActive(true);
                    }

                    // update ammoCount and change UI
                    ShotgunProjectileController.ammoCount -= 1;
                    shotgunCount.GetComponent<UnityEngine.UI.Text>().text = "Shotgun ammo: " + ShotgunProjectileController.ammoCount.ToString() + "/" + ShotgunProjectileController.maxAmmo.ToString();

                    // update next fire time to control fire rate of gun
                    shotgunNextFireTime = Time.time + ShotgunProjectileController.fireInterval;

                    // update next reload time
                    if (PlayerController2D.isGrounded)
                    {
                        nextReloadTime = Time.time + onGroundReloadInterval;
                    }
                }
            }
            else if (equippedGun == "rocket")
            {
                if (RocketProjectileController.ammoCount > 0 && Time.time > rocketNextFireTime)
                {
                    // Generate projectile for rocket
                    projectile = ObjectPooler.Instance.SpawnFromPool("rocket");
                    projectile.transform.position = FirepointPrefab.transform.position;
                    projectile.transform.eulerAngles = FirepointPrefab.transform.eulerAngles;
                    projectile.SetActive(true);

                    // update ammoCount and change UI
                    RocketProjectileController.ammoCount -= 1;
                    rocketCount.GetComponent<UnityEngine.UI.Text>().text = "Rocket ammo: " + RocketProjectileController.ammoCount.ToString() + "/" + RocketProjectileController.maxAmmo.ToString();

                    // update next fire time to control fire rate of gun
                    rocketNextFireTime = Time.time + RocketProjectileController.fireInterval;
                }
            }
        }

        if (Input.GetButtonDown("weapon 1"))
        {
            equippedGun = "shotgun";
            equippedGunText.GetComponent<UnityEngine.UI.Text>().text = "Equipped gun: Shotgun";
        }
        else if (Input.GetButtonDown("weapon 2"))
        {
            equippedGun = "rocket";
            equippedGunText.GetComponent<UnityEngine.UI.Text>().text = "Equipped gun: Rocket";
        }
    }

    public Tuple<float, float> getRecoilValues(float shotgunRecoilForce)
    {
        if (PlayerController2D.isFacingRight)
        {
            // Debug.Log("transform.eulerAngles: " + transform.eulerAngles.ToString());
            float x = shotgunRecoilForce * Mathf.Cos((transform.eulerAngles.z + 180)* Mathf.Deg2Rad);
            float y = shotgunRecoilForce * Mathf.Sin((transform.eulerAngles.z + 180) * Mathf.Deg2Rad);
            return Tuple.Create(x, y);
        }
        else
        {
            // Debug.Log("transform.eulerAngles: " + transform.eulerAngles.ToString());
            float x = shotgunRecoilForce * Mathf.Cos((transform.eulerAngles.z + 180) * Mathf.Deg2Rad) * -1;
            float y = shotgunRecoilForce * Mathf.Sin((transform.eulerAngles.z + 180) * Mathf.Deg2Rad);
            return Tuple.Create(x, y);
        }
    }
}

