using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFader : MonoBehaviour
{
    private CanvasGroup startCanvasGroup;
    private float turnTimer;
    public GameObject contextFrame;
    public GameObject startTurnOff;
    public GameObject startScreen;
    public GameObject mainCanvas;
    public GameObject otherObjects;

    private TutorialScript tutorialScript;
    //this should 100% be split up into different scripts im just a lazy fucker who wants to get it working
    void Start()
    {
        tutorialScript = mainCanvas.GetComponent<TutorialScript>();
        GameManager.paused = true;
        startCanvasGroup = startTurnOff.GetComponent<CanvasGroup>();
        StartCoroutine(UpdateCheck());
    }

    public void ContextButtonPress()
    {
        StartCoroutine(ContextMoveAway());
    }

    IEnumerator UpdateCheck()
    {
        while (turnTimer < 0.3f)
        {
            turnTimer = 0;
            while (InputRedirect.pressed)
            {
                turnTimer += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        StartCoroutine(FadeAnimation());
    }

    IEnumerator FadeAnimation()
    {
        contextFrame.SetActive(true);
        StartCoroutine(ContextMove());
        while (startCanvasGroup.alpha > 0)
        {
            startCanvasGroup.alpha -= 2f * Time.deltaTime;
            yield return null;
        }
        startTurnOff.SetActive(false);
    }

    IEnumerator ContextMove()
    {
        while (contextFrame.transform.localPosition.x < 0)
        {
            Vector3 tempLocalPosition = contextFrame.transform.localPosition;
            tempLocalPosition.x += 2400 * Time.deltaTime;
            contextFrame.transform.localPosition = tempLocalPosition;
            yield return null;
        }
    }

    IEnumerator ContextMoveAway()
    {
        while (contextFrame.transform.localPosition.x < 1600)
        {
            Vector3 tempLocalPosition = contextFrame.transform.localPosition;
            tempLocalPosition.x += 2400 * Time.deltaTime;
            otherObjects.transform.localPosition = tempLocalPosition;
            contextFrame.transform.localPosition = tempLocalPosition;
            yield return null;
        }
        mainCanvas.SetActive(true);
        startScreen.SetActive(false);
    }
}
