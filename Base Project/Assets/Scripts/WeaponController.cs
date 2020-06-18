using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform aimTransform;

    private void Awake()
    {
        aimTransform = transform.Find("Weapon");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mouseWorldPosition);

        // calc direction of aim
        Vector3 aimDirection = (mouseWorldPosition - transform.position).normalized;
        // convert to euler angles
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // apply transformation
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
    }

}
