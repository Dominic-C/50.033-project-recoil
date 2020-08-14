using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
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
    public static GameObject PauseMenuUI;
    private GameObject WeaponUI;
    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI timeDebugText;
    public Slider progressSlider;
    public Image progressFill;
    public Gradient progressColorGradient;

    // Level attributes
    private bool loadingFromSaveData = false;
    private static LevelManager Instance;
    public static int currentStage;
    public static int currentLevel = 0;
    public static string currentSceneName;
    public static bool GameIsPaused = false;
    public static bool audioIsPlaying = false;
    private bool toUpdateTime = false;
    private float timeTakenCurrentStage = 0f;

    // Audio attributes
    public AudioMixer audioMixer;
    private AudioMixerSnapshot startingSnapshot;
    private AudioMixerSnapshot pausedSnapshot;
    private AudioSource[] audioSources;

    // SaveSystem attributes
    public static Dictionary<string, float> timeTakenPerStage = new Dictionary<string, float>();
    public static List<int> EggsCollected = new List<int>();
    public static List<string> thingsPickedUp = new List<string>(); // hack method to make sure egg won't respawn

    // Player attributes
    private GameObject player;
    private Vector3 playerSpawnPosition;
    public static int unlockedGuns;  // 0 for no guns, 1 for shotgun only, 2 for shotgun + rocket, 3 for shotgun + rocket + flamethrower
    public delegate void PlayerDeath();
    public static event PlayerDeath PlayerDie;
    public static void onPlayerDeath() { PlayerDie(); }
    public static int numDeaths = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            Destroy(this);
        }

    }

    void Start()
    {
        updateStage();
        SceneManager.sceneLoaded += delegate { updateStage(); };
        SceneManager.activeSceneChanged += delegate { disablePickedUpObjects(); };
        PlayerDie += delegate { respawn(); };
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

    #region Game Operations : Play, Quit, Pause, Resume
    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        if (WeaponUI != null)
        {
            WeaponUI.SetActive(false);
        }
        FadeMixerGroup.TransitToSnapshot(pausedSnapshot);
        timeDebugText.text = timeTakenCurrentStage.ToString("n2") + " s";
        timeTakenPerStage[currentSceneName] = timeTakenCurrentStage;
        Time.timeScale = 0f;
        toUpdateTime = false;
        SaveSystem.SavePlayer();
        GameIsPaused = !GameIsPaused;
    }

    public void PlayGame()
    {
        LoadNextScene();
        unlockedGuns = (int)UnlockGunState.NO_GUN;
        SaveSystem.SavePlayer();
    }

    public void ResumeGame()
    {
        FadeMixerGroup.TransitToSnapshot(startingSnapshot);
        toUpdateTime = true;
        PauseMenuUI.SetActive(false);
        if (WeaponUI != null)
        {
            WeaponUI.SetActive(true);
        }
        SaveSystem.SavePlayer();
        Time.timeScale = 1f;
        GameIsPaused = !GameIsPaused;
    }

    public void QuitGame()
    {
        toUpdateTime = false;
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        // reset variables before returning to Main Menu
        toUpdateTime = false;
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region in-game level mechanics: updateLevel, respawn
    public void setWeaponUI()
    {
        WeaponUI = GameObject.Find("WeaponUI");
    }

    void updateStage()
    {
        currentStage = SceneManager.GetActiveScene().buildIndex;
        currentSceneName = SceneManager.GetSceneByBuildIndex(currentStage).name;
        stageNameText.text = currentSceneName;

        if (currentStage >= 1)
        {

            if (!audioIsPlaying) // play audio if it is not playing already
            {
                startingSnapshot = audioMixer.FindSnapshot("Starting");
                pausedSnapshot = audioMixer.FindSnapshot("Paused");
                audioIsPlaying = true;
                audioSources = GetComponents<AudioSource>();
                foreach (AudioSource audioSource in audioSources)
                {
                    FadeMixerGroup.TurnOffSound(audioMixer);
                    audioSource.Play();
                }

                StartCoroutine(FadeMixerGroup.StartFade(audioMixer, FadeMixerGroup.exposedBGMParams[0], 2f, 1f));
            }

            // update timeTakenPerStage
            if (!timeTakenPerStage.ContainsKey(currentSceneName) && !loadingFromSaveData)
            {
                Debug.Log("not loading from save data");
                timeTakenPerStage.Add(currentSceneName, 0f); // key may already exist if loaded from savefile.
                timeTakenCurrentStage = 0f;
                thingsPickedUp = new List<string>();
                loadingFromSaveData = false;
            }

            toUpdateTime = true;

            // set pause menu ui and time debug text
            PauseMenuUI = gameObject.transform.Find("PauseCanvas/PauseMenu").gameObject;
            if (PauseMenuUI != null)
            {
                PauseMenuUI.SetActive(false);
                Button pauseButton = GetComponentInChildren<Button>();
                pauseButton.onClick.AddListener(delegate { PauseGame(); });
                Button[] buttons = PauseMenuUI.GetComponentsInChildren<Button>();
                foreach (Button button in buttons)
                {
                    switch (button.name)
                    {
                        case "ResumeButton":
                            button.onClick.AddListener(delegate { ResumeGame(); });
                            break;
                        case "QuitButton":
                            button.onClick.AddListener(delegate { QuitGame(); });
                            break;
                    }
                }
            }

            // set player spawn position
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerSpawnPosition = player.transform.position;

            // play music, TODO: add stage transition animation
            transitToNextLevel();
        }

        if (currentStage >= 2 && WeaponUI == null)
        {
            setWeaponUI();
        }

        SaveSystem.SavePlayer();

    }

    private void transitToNextLevel()
    {
        // yes i know this line is very convoluted, but the scenemanager is not able to call GetSceneAt(index) for unloaded scenes.
        string prevSceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex - 1));

        if (currentSceneName.ElementAt(5) != prevSceneName.ElementAt(5) && prevSceneName != "MainMenu") // a hack done to see if the level the user is at changed
        {
            currentLevel += 1;
            Debug.Log("Level Changed: " + currentLevel);


            // change bgm
            audioSources[currentLevel].volume = 0.1f;
            string nextSceneMusicParam = FadeMixerGroup.exposedBGMParams[currentLevel];
            string prevSceneMusicParam = FadeMixerGroup.exposedBGMParams[currentLevel - 1];
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer, prevSceneMusicParam, 2f, 0f));
            StartCoroutine(FadeMixerGroup.StartFade(audioMixer, nextSceneMusicParam, 4f, 1f));

            // change progress slider
            if (currentSceneName.Contains("BossFight"))
            {
                progressSlider.maxValue = 2;
            } else
            {
                progressSlider.maxValue = 5;
            }

            progressSlider.value = 1;
        } 
        
        else
        {
            progressSlider.value += 1;
        }

        progressFill.color = progressColorGradient.Evaluate(progressSlider.normalizedValue);
    }

    public void respawn()
    {
        player.transform.position = playerSpawnPosition;
        numDeaths += 1;
    }
    #endregion

    #region Save, Load functions

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void loadPlayerData()
    {
        loadingFromSaveData = true;
        PlayerData playerData = SaveSystem.LoadPlayer();

        string stageToLoad = playerData.timeTakenPerStage.Keys.Last();
        timeTakenPerStage = playerData.timeTakenPerStage;
        thingsPickedUp = playerData.thingsPickedUp;
        Debug.Log("LevelManager's thingsPickedup when loading data : " + thingsPickedUp.Count);
        unlockedGuns = playerData.unlockedGuns;
        EggsCollected = playerData.eggsCollected;

        timeTakenCurrentStage = playerData.timeTakenPerStage[stageToLoad];

        // these variables need to be set again if return to Main Menu and load the game
        toUpdateTime = true;
        SceneManager.LoadScene(stageToLoad);
        if (PauseMenuUI) PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void disablePickedUpObjects()
    {
        if (thingsPickedUp.Count > 0)
        {
            // previously got pick up shit, now disable those so player won't pick up again.
            foreach (string gameObjName in thingsPickedUp)
            {
                GameObject obj = GameObject.Find(gameObjName);
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    #endregion
}