using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GunTypes
{
    public const string Rocket = "rocket";
    public const string ShotGun = "shotgun";
}

public class WeaponController : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject FirepointPrefab;

    public GameObject WeaponUI;
    private CurrWeaponUI WeaponUIData;
    public WeaponData ShotgunWeaponData;
    public WeaponData RocketWeaponData;

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

    public static bool isEnabled = false;
    public delegate void GroundReload();
    public static event GroundReload groundReload;
    public static void onGroundReload() { groundReload(); }

    private void Start()
    {
        // set up gun and delegate for PlayerController to call event to trigger
        rb2d = PlayerPrefab.GetComponent<Rigidbody2D>();
        equippedGun = GunTypes.ShotGun;
        shotgunRecoilForce = ShotgunWeaponData.recoilForce;

        // weapon UI setup
        WeaponUIData = WeaponUI.GetComponent<CurrWeaponUI>();
        WeaponUI.SetActive(true);
        setWeaponUI(equippedGun);

        groundReload += delegate { refillAmmo(); };
        isEnabled = true;
    }

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
            refillAmmo();
            setWeaponUI(equippedGun);
            if (equippedGun == GunTypes.ShotGun)
            {
                updateAmmoText(ShotgunWeaponData.maxAmmo, ShotgunWeaponData.maxAmmo);
            }
            else if (equippedGun == GunTypes.Rocket)
            {
                updateAmmoText(RocketWeaponData.maxAmmo, RocketWeaponData.maxAmmo);
            }
            
            // ensure that ammo will only be reloaded once (esp in combination with the reload when landing on the ground)
            nextReloadTime = float.MaxValue;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (equippedGun == GunTypes.ShotGun)
            {
                if (ShotgunWeaponData.ammoCount > 0 && Time.time > shotgunNextFireTime)
                {
                    // Generate projectile for shotgun
                    Tuple<float, float> recoilVals = getRecoilValues(shotgunRecoilForce);
                    float horizontalForce = rb2d.velocity.x + recoilVals.Item1;
                    float verticalForce = rb2d.velocity.y + recoilVals.Item2;
                    rb2d.AddForce(new Vector2(horizontalForce, verticalForce)); // this conflicts with player's running velocity
                    
                    projectile0 = ObjectPooler.Instance.SpawnFromPool("shotgun");
                    if (projectile0 != null)
                    {
                        projectile0.transform.position = FirepointPrefab.transform.position;
                        projectile0.transform.eulerAngles = FirepointPrefab.transform.eulerAngles + new Vector3(0,0,5);
                        projectile0.SetActive(true);
                    }
                    projectile1 = ObjectPooler.Instance.SpawnFromPool(GunTypes.ShotGun);
                    if (projectile1 != null)
                    {
                        projectile1.transform.position = FirepointPrefab.transform.position;
                        projectile1.transform.eulerAngles = FirepointPrefab.transform.eulerAngles;
                        projectile1.SetActive(true);
                    }
                    projectile2 = ObjectPooler.Instance.SpawnFromPool(GunTypes.ShotGun);
                    if (projectile2 != null)
                    {
                        projectile2.transform.position = FirepointPrefab.transform.position;
                        projectile2.transform.eulerAngles = FirepointPrefab.transform.eulerAngles + new Vector3(0,0,-5);
                        projectile2.SetActive(true);
                    }

                    // update ammoCount and change UI
                    ShotgunWeaponData.ammoCount -= 1;
                    updateAmmoText(ShotgunWeaponData.ammoCount, ShotgunWeaponData.maxAmmo);
                    
                    // update next fire time to control fire rate of gun
                    shotgunNextFireTime = Time.time + ShotgunWeaponData.fireInterval;

                    // update next reload time
                    if (PlayerController2D.isGrounded)
                    {
                        nextReloadTime = Time.time + onGroundReloadInterval;
                    }
                }
            }
            else if (equippedGun == GunTypes.Rocket)
            {
                if (RocketWeaponData.ammoCount > 0 && Time.time > rocketNextFireTime)
                {
                    // Generate projectile for rocket
                    projectile = ObjectPooler.Instance.SpawnFromPool("rocket");
                    projectile.transform.position = FirepointPrefab.transform.position;
                    projectile.transform.eulerAngles = FirepointPrefab.transform.eulerAngles;
                    projectile.SetActive(true);

                    // update ammoCount and change UI
                    RocketWeaponData.ammoCount -= 1;
                    updateAmmoText(RocketWeaponData.ammoCount, RocketWeaponData.maxAmmo);
                    
                    // update next fire time to control fire rate of gun
                    rocketNextFireTime = Time.time + RocketWeaponData.fireInterval;
                }
            }
        }

        if (Input.GetButtonDown("weapon 1"))
        {
            equippedGun = GunTypes.ShotGun;
            setWeaponUI(equippedGun);
        }
        else if (Input.GetButtonDown("weapon 2"))
        {
            equippedGun = GunTypes.Rocket;
            setWeaponUI(equippedGun);
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
    public void refillAmmo()
    {
        // reset shotgun ammo
        ShotgunWeaponData.ammoCount = ShotgunWeaponData.maxAmmo;
        // reset rocket ammo
        RocketWeaponData.ammoCount = RocketWeaponData.maxAmmo;
        // ensure that ammo will only be reloaded once (esp in combination with the passive reload when on the ground)
        nextReloadTime = float.MaxValue;

        updateAmmoText(getWeaponDataForUI(equippedGun).ammoCount, getWeaponDataForUI(equippedGun).ammoCount);
    }


    // set weapon UI methods
    public WeaponData getWeaponDataForUI(string gunName) 
    {
        if (gunName == GunTypes.ShotGun)
        {
            return ShotgunWeaponData;
        }
        else if (gunName == GunTypes.Rocket)
        {
            return RocketWeaponData;
        }
        return null;
    }

    private void setWeaponUI(string equippedGun)
    {
        Debug.Log("Setting Weapon UI from Weaponcontroller");
        if (getWeaponDataForUI(equippedGun) != null)
        {
            WeaponUIData.setWeaponData(getWeaponDataForUI(equippedGun));
            GetComponent<SpriteRenderer>().sprite = getWeaponDataForUI(equippedGun).weaponImage;
        }
    }

    private void updateAmmoText(int ammo, int maxAmmo)
    {
        Debug.Log("Ammo reduced");
        if (getWeaponDataForUI(equippedGun) != null)
        {
            WeaponUIData.updateAmmoText(ammo, getWeaponDataForUI(equippedGun).ammoCount);
        }
    }
}

