using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFill : MonoBehaviour
{
    public Image factoryCDOverlay;
    public Image trainCDOverlay;
    public Image coalCDOverlay;
    public Image oilrigCDOverlay;
    public Image solarCDOverlay;
    public Image harborCDOverlay;
    public Image natureCDOverlay;
    private float animationCountDown;

    public void buttonAnimationCall()
    {
        StartCoroutine(fillAmountAnimation());
    }

    IEnumerator fillAmountAnimation()
    {
        animationCountDown = 1 / GameManager.coolDown;
        factoryCDOverlay.fillAmount = 1;
        trainCDOverlay.fillAmount = 1;
        coalCDOverlay.fillAmount = 1;
        oilrigCDOverlay.fillAmount = 1;
        solarCDOverlay.fillAmount = 1;
        harborCDOverlay.fillAmount = 1;
        natureCDOverlay.fillAmount = 1;
        while (factoryCDOverlay.fillAmount > 0)
        {
            float animationChange = animationCountDown * GameManager.deltaTime;
            factoryCDOverlay.fillAmount -= animationChange;
            trainCDOverlay.fillAmount -= animationChange;
            coalCDOverlay.fillAmount -= animationChange;
            oilrigCDOverlay.fillAmount -= animationChange;
            solarCDOverlay.fillAmount -= animationChange;
            harborCDOverlay.fillAmount -= animationChange;
            natureCDOverlay.fillAmount -= animationChange;
            yield return null;
        }
    }
}
