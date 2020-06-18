using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int currentLevel;
    public int numLevels;
    public static LevelManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToNextLevel()
    {
        currentLevel += 1;
    }
}
