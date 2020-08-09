using UnityEngine;
using TMPro;

public class StageName : MonoBehaviour
{
    public static TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    
    public static void updateStageText(string name)
    {
        text.text = name;
    }

}
