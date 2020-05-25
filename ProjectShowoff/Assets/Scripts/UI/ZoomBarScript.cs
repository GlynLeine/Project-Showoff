using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomBarScript : MonoBehaviour
{
    [SerializeField] private Image sliderBg = null;
    [SerializeField] private Image sliderFill = null;
    private float timer;
    public void SliderValueChange()
    {
        timer = 0;
        CancelInvoke();
        InvokeRepeating("SliderAlphaChange",2.5f,0.05f);
    }

    private void SliderAlphaChange()
    {
        timer += 0.1f;
        Color bgC = sliderBg.color;
        Color fillC = sliderFill.color;
        bgC.a = 1-timer;
        fillC.a = 1-timer;
        sliderBg.color = bgC;
        sliderFill.color = fillC;
        if (timer > 1)
        {
            CancelInvoke();
        }
    }
}