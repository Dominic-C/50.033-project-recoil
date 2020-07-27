using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int ammoCount;
    public Sprite weaponImage;
    public float projectileSpeed;
    public float recoilForce;
    public float fireInterval;
    public Vector3 firePosition;
    public int maxAmmo;
    public AudioClip fireSound;
    public AudioClip switchToSound;
}
