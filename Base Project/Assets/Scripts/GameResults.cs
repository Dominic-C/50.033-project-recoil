using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResults : MonoBehaviour
{
    public GameObject EggsLayout;
    public Sprite RedEgg;
    public Sprite BlueEgg;
    public Sprite GreenEgg;

    public GameObject WeaponsLayout;
    public WeaponData shotgunData;
    public WeaponData rocketData;
    public WeaponData flamethrowerData;

    public TextMeshProUGUI TimeText;

    public Image image;
    private int numEggs = 0;
    private int numWeapons = 0;

    void OnEnable()
    {
        if (EggsLayout != null)
        {
            showEggs();
        }

        if (WeaponsLayout != null)
        {
            showWeapons();
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
                Image img = Instantiate(image, EggsLayout.transform);
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

    void showWeapons()
    {
        if (numWeapons + 1 == LevelManager.unlockedGuns)
        {
            Image img = Instantiate(image, WeaponsLayout.transform);
            switch (LevelManager.unlockedGuns)
            {
                case (int) UnlockGunState.NO_GUN:
                    break;
                case (int)UnlockGunState.SHOTGUN_ONLY:
                    img.sprite = shotgunData.weaponImage;
                    break;

                case (int) UnlockGunState.SHOTGUN_AND_ROCKET:
                    img.sprite = rocketData.weaponImage;
                    break;
                case (int)UnlockGunState.ALL_WEAPONS:
                    img.sprite = flamethrowerData.weaponImage;
                    break;
            }
            numWeapons += 1;
        }

        else
        {
            for (int j = numWeapons; j < LevelManager.unlockedGuns; j++)
            {
                Image img = Instantiate(image, WeaponsLayout.transform);
                numWeapons += 1;
                switch (numWeapons)
                {
                    case (int)UnlockGunState.NO_GUN:
                        break;
                    case (int)UnlockGunState.SHOTGUN_ONLY:
                        img.sprite = shotgunData.weaponImage;
                        break;

                    case (int)UnlockGunState.SHOTGUN_AND_ROCKET:
                        img.sprite = rocketData.weaponImage;
                        break;
                    case (int)UnlockGunState.ALL_WEAPONS:
                        img.sprite = flamethrowerData.weaponImage;
                        break;
                }
            }
        }
    }
}
