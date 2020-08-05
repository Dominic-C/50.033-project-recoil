using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string currentStage;
    public Dictionary<string, float> timeTakenPerStage;
    public int unlockedGuns;
    public List<int> eggsCollected;
    public List<string> thingsPickedUp;

    public PlayerData()
    {
        currentStage = LevelManager.currentSceneName;
        timeTakenPerStage = LevelManager.timeTakenPerStage;
        unlockedGuns = LevelManager.unlockedGuns;
        eggsCollected = LevelManager.EggsCollected;
        thingsPickedUp = LevelManager.thingsPickedUp;
    }
}

public static class SaveSystem
{
    public static string path = Application.persistentDataPath + "/lastSave.hehe";
    public static void SavePlayer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/lastSave.png");
        PlayerData data = new PlayerData(); // made based on existing LevelManager
        try
        {
            formatter.Serialize(stream, data);
        }
        catch
        {
            Debug.Log("Failed to serialize data");
        }
        finally
        {
            stream.Close();
        }
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            try
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                return data;
            }
            finally
            {
                stream.Close();
            }
        } else
        {
            Debug.Log("Path does not exist meow");
            return null;
        }
    }
}
    
