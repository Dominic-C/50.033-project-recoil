using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggEffects : MonoBehaviour
{
    public AudioSource audioSource;
    private GameObject player;

    void Start()
    {
        audioSource = GetComponents<AudioSource>()[1];
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("playSound", 2.0f, 5.0f);
    }
    void playSound()
    {
        if (!audioSource.isPlaying && Vector3.Distance(player.transform.position, gameObject.transform.position) < 5)
        {
            audioSource.Play();
        }
    }
}
