using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int level;
    public Dictionary<string, float> timeTakenPerStage;
    public List<string> mobsDestroyed;
    public int unlockedGuns;

    public PlayerData()
    {
        level = LevelManager.currentLevel;
        timeTakenPerStage = LevelManager.timeTakenPerStage;
        mobsDestroyed = LevelManager.mobsDestroyed;

        Debug.Log("Creating new PlayerData to be saved...");
        Debug.Log("Level: " + level);
        foreach (string key in timeTakenPerStage.Keys)
        {
            Debug.Log(key + ":" + timeTakenPerStage[key]);
        }
        unlockedGuns = LevelManager.unlockedGuns;
    }
}

public static class SaveSystem
{
    public static string path = Application.persistentDataPath + "/lastSave.hehe";
    public static void SavePlayer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(path, FileMode.Create);
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
    
