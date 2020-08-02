using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResults : MonoBehaviour
{
    public GameObject EggHorizontalLayout;
    public TextMeshProUGUI TimeText;
    public Sprite RedEgg;
    public Sprite BlueEgg;
    public Sprite GreenEgg;
    public Image image;

    void Start()
    {
        foreach (int egg in LevelManager.EggsCollected)
        {
            Image img = Instantiate(image, EggHorizontalLayout.transform);
            switch (egg)
            {
                case (int) EggType.RED:
                    img.sprite = RedEgg;
                    break;
                case (int) EggType.BLUE:
                    img.sprite = BlueEgg;
                    break;
                case (int) EggType.GREEN:
                    img.sprite = GreenEgg;
                    break;
            }
        }

        float totalTimeTaken = LevelManager.timeTakenPerStage.Values.Sum();
        TimeText.text = totalTimeTaken.ToString("n2") + " s";
    }
}
