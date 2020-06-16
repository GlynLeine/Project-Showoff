using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    private CanvasGroup startCanvasGroup;
    private float turnTimer;
    public GameObject mainCanvas;
    
    void Start()
    {
        startCanvasGroup = this.GetComponent<CanvasGroup>();
        StartCoroutine(updateCheck());
    }

    IEnumerator updateCheck()
    {
        while (turnTimer < 0.4f)
        {
            turnTimer = 0;
            while (InputRedirect.pressed)
            {
                turnTimer += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        StartCoroutine(fadeAnimation());
    }

    IEnumerator fadeAnimation()
    {
        while (startCanvasGroup.alpha > 0)
        {
            startCanvasGroup.alpha -= 1f * Time.deltaTime;
            yield return null;
        }
        mainCanvas.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
