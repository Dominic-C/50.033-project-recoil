using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int currentLevel;
    public Vector3 playerSpawnPosition;

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
        currentDistance = 0;
    }

    // Progress UI related functions
    private float maxDistance;
    private float currentDistance; // affected by how much player moves from starting point of the level.
    public Slider progressSlider; // UI to show how much the player has progressed in the level.
    public Image progressFill;
    public Gradient progressColorGradient;

    public void SetLevelProgress (float distance)
    {
        if (distance > currentDistance)
        {
            Debug.Log("setting level progress");
            currentDistance = distance;
            progressSlider.value = currentDistance;
            progressFill.color = progressColorGradient.Evaluate(progressSlider.normalizedValue);
        }
    }

    public void SetMaxDistance(float maxDistance)
    {

        progressSlider.maxValue = maxDistance;
    }

    void GoToNextLevel()
    {
        currentLevel += 1;
        currentDistance = 0;
        progressSlider.value = currentDistance;
        progressFill.color = progressColorGradient.Evaluate(currentDistance / maxDistance);
    }

}
