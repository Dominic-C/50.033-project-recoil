using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public GameObject LoadInterface;
    public Image lastSaveImage;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI lastPlayedText;
    // Start is called before the first frame update
    void Start()
    {
        LoadInterface.SetActive(false);
        if (!File.Exists(SaveSystem.path))
        {
            gameObject.SetActive(false);
        }
    }

    public void hideInterface()
    {
        LoadInterface.SetActive(false);
    }

    public void ShowLoadInterface()
    {
        // Read the data from the file
        byte[] data = File.ReadAllBytes(Application.persistentDataPath + "/" + "lastSave.png");

        // Create the texture
        Texture2D screenshotTexture = new Texture2D(900, 900, TextureFormat.RGB24, false);

        // Load the image
        screenshotTexture.LoadImage(data);

        // Create a sprite
        Sprite screenshotSprite = Sprite.Create(screenshotTexture, new Rect(0, 0, Screen.width, Screen.height-1), new Vector2(0.5f, 0.5f));

        // Set the sprite to the screenshotPreview
        lastSaveImage.sprite = screenshotSprite;


        PlayerData playerData = SaveSystem.LoadPlayer();

        stageText.text = playerData.timeTakenPerStage.Keys.Last();
        lastPlayedText.text = File.GetCreationTime(SaveSystem.path).ToString();

        LoadInterface.SetActive(true);
    }

    public void DeleteSaveData()
    {
        File.Delete(SaveSystem.path);
        LoadInterface.SetActive(false);
        gameObject.SetActive(false);
    }
}
