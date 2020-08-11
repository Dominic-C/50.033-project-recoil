using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public Button skipButton;
    public bool addBenedict = false;
    
    void Start(){
        OnEnable();
    }
    public static void OnEnable()
    {
        LevelManager.EggsCollected.Add(4);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
