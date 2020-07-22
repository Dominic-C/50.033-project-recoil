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

public enum EggType
{
    RED,
    GREEN,
    BLUE
}

public class Pickup : MonoBehaviour
{
    public PickupType type;
    public UnityEvent onEnter;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            gameObject.SetActive(false); // to be replaced with some animation of disappearing
            if (onEnter != null) onEnter.Invoke();
        }
    }

    public void showUI(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void getShotgun()
    {
        LevelManager.unlockedGuns = (int) UnlockGunState.SHOTGUN_ONLY;
    }

    public void getRocket()
    {
        LevelManager.unlockedGuns = (int)UnlockGunState.SHOTGUN_AND_ROCKET;
    }

    public void getFlamethrower()
    {
        LevelManager.unlockedGuns = (int)UnlockGunState.ALL_WEAPONS;
    }

    
    public void getEgg(EggType type)
    {
        LevelManager.EggsCollected.Add((int) type);
    }

}
