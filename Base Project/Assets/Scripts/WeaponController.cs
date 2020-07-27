using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class GunTypes
{
    public const string Rocket = "rocket";
    public const string Shotgun = "shotgun";
    public const string Flamethrower = "flamethrower";
}

public class WeaponController : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject FirepointPrefab;
    public GameObject FlamethrowerParticleSystem;
    public GameObject PlayerArm;

    private GameObject WeaponUI;
    private CurrWeaponUI WeaponUIData;
    public WeaponData ShotgunWeaponData;
    public WeaponData RocketWeaponData;
    public WeaponData FlamethrowerWeaponData;

    private GameObject projectile;
    private GameObject projectile0;
    private GameObject projectile1;
    private GameObject projectile2;
    private ParticleSystem flamethrowerProjectile;

    // private Transform aimTransform;
    private Rigidbody2D rb2d;

    private string equippedGun;
    private AudioSource weaponFireAudioSource;
    private AudioSource weaponSwitchAudioSource;

    private float shotgunNextFireTime = 0.0f;  // in seconds
    private float rocketNextFireTime = 0.0f;
    private float flamethrowerNextFireTime = 0.0f;

    private float onGroundReloadInterval = 3.0f;
    public static float nextReloadTime = 0.0f;

    public float xCorrection = 0.70f;
    public float yCorrection = 0.9f;

    public static bool isEnabled = false;
    public delegate void GroundReload();
    public static event GroundReload groundReload;
    public static void onGroundReload() { groundReload(); }

    private Animator animator;

    private void OnEnable()
    {
        isEnabled = true;
    }

    private void onDisable()
    {
        isEnabled = false;
    }

    private void Start()
    {
        // set up gun and delegate for PlayerController to call event to trigger
        rb2d = PlayerPrefab.GetComponent<Rigidbody2D>();
        equippedGun = GunTypes.Shotgun;

        // weapon UI setup
        findWeaponUI();

        groundReload += delegate { refillAmmo(); };
        AudioSource[] sources = GetComponents<AudioSource>();
        weaponFireAudioSource = sources[0];
        weaponSwitchAudioSource = sources[1];
        flamethrowerProjectile = FlamethrowerParticleSystem.GetComponent<ParticleSystem>();
    }

    public void findWeaponUI()
    {
        WeaponUI = GameObject.Find("WeaponUI");
        if (WeaponUI != null)
        {
            WeaponUIData = WeaponUI.GetComponent<CurrWeaponUI>();
            setWeaponUI(equippedGun);
        }
    }

    void FixedUpdate()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // calc direction of aim
        Vector3 aimDirection = (mouseWorldPosition - PlayerArm.transform.position).normalized;

        // apply transformation based on whether its flipped
        if (PlayerController2D.isFacingRight)
        {
            // convert to euler angles
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            PlayerArm.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, angle);
        }
        else
        {
            // convert to euler angles
            float angle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
            angle = angle + 90;  // i forgot the math, not sure why need to +90 degrees in this case
            PlayerArm.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, angle);
        }

        if (Time.time > nextReloadTime)
        {
            refillAmmo();
        }
    }

    void Update()
    {
        // only take in input when game is not paused
        if (!LevelManager.GameIsPaused)
        {
            bool shootClicked = Input.GetButton("Fire1");
            if (shootClicked)
            {
                switch (equippedGun)
                {
                    case (GunTypes.Flamethrower):
                        // Check if we have Flamethrower Ammo and can fire it.
                        if (FlamethrowerWeaponData.ammoCount > 0 && Time.time > flamethrowerNextFireTime)
                        {
                            // Generate Force, set ammo count, update UI
                            shootRecoilForce(FlamethrowerWeaponData.recoilForce);
                            weaponFireAudioSource.PlayOneShot(FlamethrowerWeaponData.fireSound);
                            FlamethrowerWeaponData.ammoCount -= 1;
                            if (WeaponUI != null) setWeaponUI(equippedGun);

                            // update next reload time
                            flamethrowerNextFireTime = Time.time + FlamethrowerWeaponData.fireInterval;
                            if (PlayerController2D.isGrounded) nextReloadTime = Time.time + onGroundReloadInterval;

                            // start flame animation
                            if (!flamethrowerProjectile.isPlaying) flamethrowerProjectile.Play();
                        }
                        break;
                    case (GunTypes.Shotgun):
                        // Check if we have Shotgun Ammo and can fire it.
                        if (ShotgunWeaponData.ammoCount > 0 && Time.time > shotgunNextFireTime)
                        {
                            shootRecoilForce(ShotgunWeaponData.recoilForce);

                            // TODO: MORE REFACTORING NEEDED.
                            // I REFUSE TO BELIEVE THIS CAN'T BE FIXED.
                            projectile0 = ObjectPooler.Instance.SpawnFromPool(GunTypes.Shotgun);
                            if (projectile0 != null)
                            {
                                projectile0.transform.position = FirepointPrefab.transform.position;
                                projectile0.transform.eulerAngles = FirepointPrefab.transform.eulerAngles + new Vector3(0, 0, 5);
                                projectile0.SetActive(true);
                            }
                            projectile1 = ObjectPooler.Instance.SpawnFromPool(GunTypes.Shotgun);
                            if (projectile1 != null)
                            {
                                projectile1.transform.position = FirepointPrefab.transform.position;
                                projectile1.transform.eulerAngles = FirepointPrefab.transform.eulerAngles;
                                projectile1.SetActive(true);
                            }
                            projectile2 = ObjectPooler.Instance.SpawnFromPool(GunTypes.Shotgun);
                            if (projectile2 != null)
                            {
                                projectile2.transform.position = FirepointPrefab.transform.position;
                                projectile2.transform.eulerAngles = FirepointPrefab.transform.eulerAngles + new Vector3(0, 0, -5);
                                projectile2.SetActive(true);
                            }

                            // update ammoCount and change UI
                            ShotgunWeaponData.ammoCount -= 1;
                            weaponFireAudioSource.PlayOneShot(ShotgunWeaponData.fireSound);
                            if (WeaponUI != null) setWeaponUI(equippedGun);

                            // update next fire time to control fire rate of gun
                            shotgunNextFireTime = Time.time + ShotgunWeaponData.fireInterval;

                            // update next reload time
                            if (PlayerController2D.isGrounded) nextReloadTime = Time.time + onGroundReloadInterval;
                        }
                        break;
                    case (GunTypes.Rocket):
                        // Check if we have Rocket Ammo and can fire it.
                        if (RocketWeaponData.ammoCount > 0 && Time.time > rocketNextFireTime)
                        {
                            // Generate projectile for rocket
                            projectile = ObjectPooler.Instance.SpawnFromPool("rocket");
                            projectile.transform.position = FirepointPrefab.transform.position;
                            projectile.transform.eulerAngles = FirepointPrefab.transform.eulerAngles;
                            projectile.SetActive(true);
                            

                            // update ammoCount and change UI
                            RocketWeaponData.ammoCount -= 1;
                            weaponFireAudioSource.PlayOneShot(RocketWeaponData.fireSound);

                            // update next fire time to control fire rate of gun
                            nextReloadTime = Time.time + RocketWeaponData.fireInterval;

                            // update next reload time
                            if (PlayerController2D.isGrounded)
                            {
                                nextReloadTime = Time.time + onGroundReloadInterval;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // stop flame animation
                if (flamethrowerProjectile.isPlaying)
                {
                    flamethrowerProjectile.Stop();
                }
            }

            if (Input.GetButtonDown("weapon 1") && LevelManager.unlockedGuns >= 1)
            {
                equippedGun = GunTypes.Shotgun;
                FirepointPrefab.transform.localPosition = ShotgunWeaponData.firePosition;
                weaponSwitchAudioSource.PlayOneShot(ShotgunWeaponData.switchToSound);
                
            }
            else if (Input.GetButtonDown("weapon 2") && LevelManager.unlockedGuns >= 2)
            {
                equippedGun = GunTypes.Rocket;
                FirepointPrefab.transform.localPosition = RocketWeaponData.firePosition;
                weaponSwitchAudioSource.PlayOneShot(RocketWeaponData.switchToSound);
            }
            else if (Input.GetButtonDown("weapon 3") && LevelManager.unlockedGuns >= 3)
            {
                equippedGun = GunTypes.Flamethrower;
                FirepointPrefab.transform.localPosition = FlamethrowerWeaponData.firePosition;
                weaponSwitchAudioSource.clip = FlamethrowerWeaponData.switchToSound;
            }

            if (WeaponUI != null) setWeaponUI(equippedGun);
        }

    }

    public void shootRecoilForce(float recoilForce)
    {
        float x, y;
        if (PlayerController2D.isFacingRight)
        {
            // Debug.Log("transform.eulerAngles: " + transform.eulerAngles.ToString());
            x = recoilForce * xCorrection * Mathf.Cos((transform.eulerAngles.z + 180) * Mathf.Deg2Rad);
            y = recoilForce * yCorrection * Mathf.Sin((transform.eulerAngles.z + 180) * Mathf.Deg2Rad);
        }
        else
        {
            // Debug.Log("transform.eulerAngles: " + transform.eulerAngles.ToString());
            x = recoilForce * xCorrection * Mathf.Cos((transform.eulerAngles.z + 180) * Mathf.Deg2Rad) * -1;
            y = recoilForce * yCorrection * Mathf.Sin((transform.eulerAngles.z + 180) * Mathf.Deg2Rad);
        }
        rb2d.velocity = new Vector2(0, 0);
        rb2d.AddForce(new Vector2(x, y));
        return;
    }
    public void refillAmmo()
    {
        ShotgunWeaponData.ammoCount = ShotgunWeaponData.maxAmmo;
        RocketWeaponData.ammoCount = RocketWeaponData.maxAmmo;
        FlamethrowerWeaponData.ammoCount = FlamethrowerWeaponData.maxAmmo;
        // ensure that ammo will only be reloaded once (esp in combination with the passive reload when on the ground)
        nextReloadTime = float.MaxValue;

        if (WeaponUI != null) setWeaponUI(equippedGun);
    }


    // set weapon UI methods
    public WeaponData getWeaponDataForUI(string gunName)
    {
        switch (gunName)
        {
            case GunTypes.Shotgun:
                return ShotgunWeaponData;
            case GunTypes.Rocket:
                return RocketWeaponData;
            case GunTypes.Flamethrower:
                return FlamethrowerWeaponData;
            default:
                return null;
        }
    }

    private void setWeaponUI(string equippedGun)
    {
        if (getWeaponDataForUI(equippedGun) != null)
        {
            WeaponData weaponData = getWeaponDataForUI(equippedGun);
            WeaponUIData.setWeaponData(weaponData);
            GetComponent<SpriteRenderer>().sprite = weaponData.weaponImage;
        }
    }
}


