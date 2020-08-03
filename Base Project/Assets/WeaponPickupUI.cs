using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPickupUI : MonoBehaviour
{
    // Start is called before the first frame update
    public WeaponData weaponData;
    public Image weaponImage;
    public TextMeshProUGUI weaponNameText;


    void Start()
    {
        weaponImage.sprite = weaponData.weaponImage;
        weaponNameText.text = weaponData.weaponName;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
