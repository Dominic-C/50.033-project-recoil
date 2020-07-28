using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] batteries;
    private int energyLevel;

    void Start()
    {
        energyLevel = batteries.Length;
    }

    // Update is called once per frame
    void Update()
    {
        energyLevel = 0;
        for (int i =0; i < batteries.Length; i++)
        {
            if (!batteries[i].GetComponent<BatteryStatus>().isHit)
            {
                energyLevel += 1;
            }
        }

        if (energyLevel <= 0)
        {
            KillSelf();
        }
    }

    private void KillSelf()
    {
        Destroy(gameObject);
    }
}
