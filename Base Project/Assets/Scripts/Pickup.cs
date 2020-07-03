using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Weapon, 
    Hint,
    EasterEgg
}

public class Pickup : MonoBehaviour
{
    public PickupType type;
    public GameObject linkedObject;

    void OnTriggerEnter2D()
    {
        linkedObject.SetActive(true);
        gameObject.SetActive(false); // to be replaced with some animation of disappearing
    }

    void Awake()
    {
        linkedObject.SetActive(false);
    }

}
