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
    private int numEggs = 0;

    void OnEnable()
    {
        if (EggHorizontalLayout != null)
        {
            showEggs();
        }

        if (TimeText)
        {
            float totalTimeTaken = LevelManager.timeTakenPerStage.Values.Sum();
            TimeText.text = totalTimeTaken.ToString("n2") + " s";
        }

    }

    void showEggs()
    {
        if (numEggs != LevelManager.EggsCollected.Count)
        {
            for (int j = numEggs; j < LevelManager.EggsCollected.Count; j++ )
            {
                Image img = Instantiate(image, EggHorizontalLayout.transform);
                switch (LevelManager.EggsCollected[j])
                {
                    case (int)EggType.RED:
                        img.sprite = RedEgg;
                        break;
                    case (int)EggType.BLUE:
                        img.sprite = BlueEgg;
                        break;
                    case (int)EggType.GREEN:
                        img.sprite = GreenEgg;
                        break;
                }
                numEggs += 1;
            }
        }
    }
}
