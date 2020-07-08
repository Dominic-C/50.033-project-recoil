using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int ammoCount;
    public Sprite weaponImage;
}
