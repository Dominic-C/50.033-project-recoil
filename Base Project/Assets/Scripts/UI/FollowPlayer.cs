using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject Player;
    private Vector3 playerPos;
    private Vector3 offsetPos = new Vector3(0, 0, -10);
    // Start is called before the first frame update
    void Start()
    {
        playerPos = Player.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = Player.GetComponent<Transform>().position;
        transform.position = playerPos + offsetPos;
    }
}
