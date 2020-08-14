using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrWeaponUI : MonoBehaviour
{
    public TextMeshProUGUI AmmoCounter;
    public TextMeshProUGUI GunName;
    public Image GunSprite;

    public void setWeaponData(WeaponData data)
    {
        AmmoCounter.text = data.ammoCount.ToString() + " / " + data.maxAmmo.ToString();
        GunName.text = data.weaponName;
        GunSprite.sprite = data.weaponImage;
        //SetReloadTime(data.maxAmmo);
    }

    void Update()
    {
        if (toUpdateTime)
        {
            timeElapsed += Time.deltaTime;
            SetReloadProgress(timeElapsed);
        }

        if (timeElapsed > reloadTime)
        {
            StopAmmoRefillCooldown();
        }
    }

    #region Progress Slider UI Functions

    // Weapon Reload UI related functions
    private float timeElapsed;
    private bool toUpdateTime;
    private float reloadTime;
    public Slider progressSlider;
    public Image progressFill;
    public Gradient progressColorGradient;


    public void SetReloadProgress(float timeElapsed)
    {
        progressSlider.value = timeElapsed;
        progressFill.color = progressColorGradient.Evaluate(progressSlider.normalizedValue);
    }

    public void SetReloadTime(float reloadTime)
    {
        this.reloadTime = reloadTime;
        progressSlider.maxValue = reloadTime;
    }

    public void StartAmmoRefillCooldown()
    {
        toUpdateTime = true;
        timeElapsed = 0;
        SetReloadProgress(timeElapsed);
    }

    public void StopAmmoRefillCooldown()
    {
        timeElapsed = 0;
        SetReloadProgress(timeElapsed);
        toUpdateTime = false;
    }
    #endregion  
}
