using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public void showUI(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
    public void hideUI(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
