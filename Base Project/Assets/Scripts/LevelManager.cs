using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum UnlockGunState
{
    NO_GUN,
    SHOTGUN_ONLY,
    SHOTGUN_AND_ROCKET,
    ALL_WEAPONS
}

public class LevelManager : MonoBehaviour
{
    // UI gameObjects, found dynamically everytime loadNextScene is called.
    private GameObject PauseMenuUI;
    public bool debugMode = false;
    private TextMeshProUGUI timeDebugText;

    // Level attributes
    public static LevelManager Instance;
    public static int currentLevel;
    private string currentSceneName;
    public static bool GameIsPaused = false;
    public static bool audioIsPlaying = false;
    private bool toUpdateTime = false;
    private float timeTakenCurrentStage = 0f;

    // SaveSystem attributes
    public static Dictionary<string, float> timeTakenPerStage = new Dictionary<string, float>();
    public static List<int> EggsCollected = new List<int>();
    public static List<string> mobsDestroyed; // TODO: monsters onDestroy should add to this list.

    // Player attributes
    private GameObject player;
    private Vector3 playerSpawnPosition;
    public static int unlockedGuns;  // 0 for no guns, 1 for shotgun only, 2 for shotgun + rocket, 3 for shotgun + rocket + flamethrower
    public delegate void PlayerDeath();
    public static event PlayerDeath PlayerDie;
    public static void onPlayerDeath() { PlayerDie(); }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            
        } else
        {
            Destroy(this);
        }
        
    }

    void Start()
    {
        updateLevel();
        SceneManager.sceneLoaded += delegate { updateLevel(); };
        PlayerDie += delegate { respawn(); };
        if (!audioIsPlaying)
        {
            print("playing audio");
            audioIsPlaying = true;
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused) PauseGame();
            else ResumeGame();
        }

        if (toUpdateTime)
        {
            timeTakenCurrentStage += Time.deltaTime;
            timeTakenPerStage[currentSceneName] = timeTakenCurrentStage;
        }
    }

    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        if (debugMode)
        {
            timeDebugText.text = "Time: " + timeTakenCurrentStage;
        }
        timeTakenPerStage[currentSceneName] = timeTakenCurrentStage;
        Time.timeScale = 0f;
        toUpdateTime = false;
        GameIsPaused = !GameIsPaused;
        SaveSystem.SavePlayer();
    }

    public void PlayGame()
    {
        LoadNextScene();
        unlockedGuns = (int)UnlockGunState.NO_GUN;
        SaveSystem.SavePlayer();
    }

    public void ResumeGame()
    {
        toUpdateTime = true;
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = !GameIsPaused;
        SaveSystem.SavePlayer();
    }

    public void QuitGame()
    {
        toUpdateTime = false;
        Debug.Log("QUIT!");
        Application.Quit();
    }

    void updateLevel()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        mobsDestroyed = new List<string>();
        Debug.Log("now level: " + currentLevel);

        if (currentLevel >= 1)
        {
            currentSceneName = SceneManager.GetSceneByBuildIndex(currentLevel).name;
            if (!timeTakenPerStage.ContainsKey(currentSceneName))
            {
                timeTakenPerStage.Add(currentSceneName, 0f); // key may already exist if loaded from savefile.
            }

            toUpdateTime = true;

            // set pause menu ui and time debug text
            PauseMenuUI = GameObject.Find("PauseMenu");
            if (PauseMenuUI != null) PauseMenuUI.SetActive(false);
            
            if (debugMode)
            {
                timeDebugText = PauseMenuUI.gameObject.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
                timeDebugText.gameObject.SetActive(true);
            }

            // set player spawn position
            player = GameObject.FindGameObjectWithTag("Player");
            print(player);
            if (player != null) playerSpawnPosition = player.transform.position;
            print(playerSpawnPosition);
        }
        
        if (!timeTakenPerStage.ContainsKey(currentSceneName))
        {
            timeTakenCurrentStage = 0f; // this value may be loaded from savefile.
        }

        SaveSystem.SavePlayer();
    }

    public void respawn()
    {
        Debug.Log("Respawning");
        player.transform.position = playerSpawnPosition;
    }

    #region Save, Load functions

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void loadPlayerData()
    {
        Debug.Log("loading player data");
        PlayerData playerData = SaveSystem.LoadPlayer();

        int levelToLoad = playerData.level;
        timeTakenPerStage = playerData.timeTakenPerStage;
        SceneManager.LoadScene(levelToLoad);
        unlockedGuns = playerData.unlockedGuns;
        foreach (string key in timeTakenPerStage.Keys)
        {
            Debug.Log(key+":"+ timeTakenPerStage[key]);
        }

        timeTakenCurrentStage = playerData.timeTakenPerStage[timeTakenPerStage.Keys.ToList().Last()];

        /* TODO: Accomodate Checkpoint implementation
         * Mobs that are destroyed should stay dead when reloading from a checkpoint.
         * Pickups that are already picked up should not be shown.
         */
        if (mobsDestroyed != null)
        {
            foreach (string mobObjName in mobsDestroyed)
            {
                Destroy(GameObject.Find(mobObjName));
            }
        }

    }

    #endregion

    #region Progress Slider UI Functions (currently unused)

    // Progress UI related functions
    private float maxDistance = 0;
    private float currentDistance = 0; // affected by how much player moves from starting point of the level.
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

    void fillProgressSlider()
    {
        currentDistance = 0;
        progressSlider.value = currentDistance;
        progressFill.color = progressColorGradient.Evaluate(currentDistance / maxDistance);
    }

    #endregion  
}
