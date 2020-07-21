using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrWeaponUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI AmmoCounter;
    public TextMeshProUGUI GunName;
    public Image GunSprite;

    public void setWeaponData(WeaponData data)
    {
        Debug.Log("Setting weapon data for UI");
        AmmoCounter.text = data.ammoCount.ToString() + " / " + data.maxAmmo.ToString();
        GunName.text = data.weaponName;
        GunSprite.sprite = data.weaponImage;
    }

    // junde: i'm replacing all uses of updateAmmoText with setWeaponData
    // public void updateAmmoText(int currAmmoCount, int totalAmmoCount)
    // {
    //     AmmoCounter.text = currAmmoCount.ToString() + " / " + totalAmmoCount.ToString();
    // }
}
