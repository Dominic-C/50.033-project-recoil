using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class fadeInOutUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public Vector2 offset;
    public bool fadeIn;
    public bool fadeOut;
    public bool persistThroughScenes = false;
    public float waitTimeBeforeAnimation = 2.0f;
    public float fadeOutDuration = 2.0f;

    void Start()
    {
        StartCoroutine(WaitBeforeAnimation());
        if (persistThroughScenes)
        {
            SceneManager.activeSceneChanged += delegate { StartCoroutine(showUI()); };
        }
    }

    IEnumerator showUI()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if (fadeIn)
            {
                StartCoroutine(fadeInAndOut(t.gameObject, true, 1f));
            }
        }
        yield return new WaitForSeconds(waitTimeBeforeAnimation);
        yield return StartCoroutine(WaitBeforeAnimation());
    }

    IEnumerator WaitBeforeAnimation()
    {
        yield return new WaitForSeconds(waitTimeBeforeAnimation);
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if (fadeOut)
            {
                StartCoroutine(fadeInAndOut(t.gameObject, false, fadeOutDuration));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + new Vector3(offset.x, offset.y, 0);
        }
    }

    IEnumerator fadeInAndOut(GameObject objectToFade, bool fadeIn, float duration)
    {
        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0;
            b = 1;
        }
        else
        {
            a = 1;
            b = 0;
        }

        int mode = 0;
        Color currentColor = Color.clear;
        float currentAlpha = 0;

        SpriteRenderer tempSPRenderer = objectToFade.GetComponent<SpriteRenderer>();
        Image tempImage = objectToFade.GetComponent<Image>();
        RawImage tempRawImage = objectToFade.GetComponent<RawImage>();
        MeshRenderer tempRenderer = objectToFade.GetComponent<MeshRenderer>();
        Text tempText = objectToFade.GetComponent<Text>();
        CanvasGroup tempCanvasGroup = objectToFade.GetComponent<CanvasGroup>(); 

        //Check if this is a Sprite
        if (tempSPRenderer != null)
        {
            currentColor = tempSPRenderer.color;
            mode = 0;
        }
        //Check if Image
        else if (tempImage != null)
        {
            currentColor = tempImage.color;
            mode = 1;
        }
        //Check if RawImage
        else if (tempRawImage != null)
        {
            currentColor = tempRawImage.color;
            mode = 2;
        }
        //Check if Text 
        else if (tempText != null)
        {
            currentColor = tempText.color;
            mode = 3;
        }
        //Check if 3D Object
        else if (tempRenderer != null)
        {
            currentColor = tempRenderer.material.color;
            mode = 4;

            //ENABLE FADE Mode on the material if not done already
            tempRenderer.material.SetFloat("_Mode", 2);
            tempRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tempRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tempRenderer.material.SetInt("_ZWrite", 0);
            tempRenderer.material.DisableKeyword("_ALPHATEST_ON");
            tempRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            tempRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            tempRenderer.material.renderQueue = 3000;
        }
        else if( tempCanvasGroup != null)
        {
            currentAlpha = tempCanvasGroup.alpha;
            mode = 5;
        }
        else
        {
            yield break;
        }

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = 0f;
            if (duration == 0)
            {
                alpha = 0f;
            } else
            {
                alpha = Mathf.Lerp(a, b, counter / duration);
            }

            switch (mode)
            {
                case 0:
                    tempSPRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 1:
                    tempImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 2:
                    tempRawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 3:
                    tempText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 4:
                    tempRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
                    break;
                case 5:
                    tempCanvasGroup.alpha = alpha;
                    break;
            }
            yield return null;
        }
    }
}
