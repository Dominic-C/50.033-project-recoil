using System.Collections;
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
    public bool canRespawn = false;
    public float respawnDelay = 0.0f;
    private float timer = 0.0f;
    private AudioSource pickupSound;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            pickupSound = GetComponent<AudioSource>();
            if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);
            
            if (!canRespawn)
            {
                LevelManager.thingsPickedUp.Add(gameObject.name);
                gameObject.SetActive(false); // to be replaced with some animation of disappearing
            }
            else
            {
                // In this case, we have 'respawning powerups' which can reappear again when the time is right.
                GetComponent<Renderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(CountdownRespawn());
            }
            if (onEnter != null) onEnter.Invoke();
        }
    }

    IEnumerator CountdownRespawn()
    {
        timer = respawnDelay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        respawnPickup();
    }


    public void showUI(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void hideUI(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void getShotgun()
    {
        LevelManager.unlockedGuns = (int)UnlockGunState.SHOTGUN_ONLY;
    }

    public void getRocket()
    {
        LevelManager.unlockedGuns = (int)UnlockGunState.SHOTGUN_AND_ROCKET;
    }

    public void getFlamethrower()
    {
        LevelManager.unlockedGuns = (int)UnlockGunState.ALL_WEAPONS;
    }


    public void getEgg(int eggType)
    {
        LevelManager.EggsCollected.Add(eggType);
    }

    void respawnPickup()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }

}
