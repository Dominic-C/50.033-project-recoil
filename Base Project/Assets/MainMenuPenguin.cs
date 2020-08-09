using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPenguin : MonoBehaviour
{
    public Sprite Reaction;
    private Sprite originalExpression;
    private Image renderedImage;

    void Start()
    {
        Debug.Log("MainMenu Penguin!");
        renderedImage = GetComponent<Image>();
        originalExpression = renderedImage.sprite;
    }

    public void changeExpression()
    {
        Debug.Log("Changing Expression");
        StartCoroutine(ChangeExpression());

    }

    IEnumerator ChangeExpression()
    {
        renderedImage.sprite = Reaction;
        yield return new WaitForSeconds(1f);
        renderedImage.sprite = originalExpression;
    }
}
