using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isHit;
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsLeft(float CurrentPoint, float TargetPoint)
    {
        if (CurrentPoint < TargetPoint)
        {
            return true;
        }
        return false;
    }
    public bool IsRight(float CurrentPoint, float TargetPoint)
    {
        if (CurrentPoint > TargetPoint)
        {
            return true;
        }
        return false;
    }

    // can add IsAbove or IsBelow
}
