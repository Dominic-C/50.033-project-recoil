using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;

public enum PickupType
{
    Weapon, 
    Hint,
    EasterEgg,
    AmmoRefill
}

public class Pickup : MonoBehaviour
{
    public PickupType type;
    public GameObject linkedObject;
    public UnityEvent onEnter;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (linkedObject) linkedObject.SetActive(true);
            gameObject.SetActive(false); // to be replaced with some animation of disappearing
            if (onEnter != null) onEnter.Invoke();
        }
    }

    void Awake()
    {
        if (linkedObject != null) linkedObject.SetActive(false);
    }

}
