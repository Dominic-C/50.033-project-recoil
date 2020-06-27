using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject FirepointPrefab;

    private GameObject projectile;

    private Transform aimTransform;
    private Rigidbody2D rb2d;

    private string projectileType;
    
    [SerializeField]
    public float recoilForce;

    private void Awake()
    {
        aimTransform = transform;
    }

    private void Start()
    {
        rb2d = PlayerPrefab.GetComponent<Rigidbody2D>();
        projectileType = "gunProjectile";
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
            aimTransform.eulerAngles = new Vector3(0, aimTransform.eulerAngles.y, angle);
        }
        else
        {
            // convert to euler angles
            float angle = Mathf.Atan2(aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
            aimTransform.eulerAngles = new Vector3(0, aimTransform.eulerAngles.y, angle+90);  // i forgot the math, not sure why need to +90 degrees in this case
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Tuple<float, float> recoilVals = getRecoilValues(recoilForce);
            float horizontalForce = rb2d.velocity.x + recoilVals.Item1;
            float verticalForce = rb2d.velocity.y + recoilVals.Item2;
            Debug.Log("horizontalForce: " + horizontalForce.ToString() + ", verticalForce: " + verticalForce.ToString());
            rb2d.AddForce(new Vector2(horizontalForce, verticalForce));

            if (projectileType == "gunProjectile")
            {
                projectile = ObjectPooler.Instance.SpawnFromPool("gunProjectile");
                projectile.transform.position = FirepointPrefab.transform.position;
                projectile.transform.rotation = FirepointPrefab.transform.rotation;
                projectile.SetActive(true);
            }
        }
    }

    public Tuple<float, float> getRecoilValues(float recoilForce)
    {
        if (PlayerController2D.isFacingRight)
        {
            Debug.Log("aimTransform.eulerAngles: " + aimTransform.eulerAngles.ToString());
            float x = recoilForce * Mathf.Cos((aimTransform.eulerAngles.z + 180)* Mathf.Deg2Rad);
            float y = recoilForce * Mathf.Sin((aimTransform.eulerAngles.z + 180) * Mathf.Deg2Rad);
            return Tuple.Create(x, y);
        }
        else
        {
            Debug.Log("aimTransform.eulerAngles: " + aimTransform.eulerAngles.ToString());
            float x = recoilForce * Mathf.Cos((aimTransform.eulerAngles.z + 180) * Mathf.Deg2Rad) * -1;
            float y = recoilForce * Mathf.Sin((aimTransform.eulerAngles.z + 180) * Mathf.Deg2Rad);
            return Tuple.Create(x, y);
        }
    }
}
