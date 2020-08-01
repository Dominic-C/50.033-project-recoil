using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public Button skipButton;
    
    void Start(){
        OnEnable();
    }
    public static void OnEnable()
    {
        Debug.Log("Going to next scene");
    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
