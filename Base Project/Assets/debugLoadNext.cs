using UnityEngine;

public class debugLoadNext : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            LevelManager.LoadNextScene();
        }       
    }
}
