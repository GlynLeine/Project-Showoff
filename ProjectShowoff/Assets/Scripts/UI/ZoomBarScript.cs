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
        InvokeRepeating("TimerRepeat", 2.5f, 0.1f);
    }

    private void TimerRepeat()
    {
        timer += 0.1f;
        Color bgC = sliderBg.color;
        Color fillC = sliderFill.color;
        bgC.a = 1 - 0.025f * timer;
        fillC.a = 1 - 0.025f * timer;
        sliderBg.color = bgC;
        sliderFill.color = fillC;
        if (timer > 40)
        {
            CancelInvoke("TimerRepeat");
        }
    }

    public void SliderAlphaChange()
    {
        Color bgC = sliderBg.color;
        Color fillC = sliderFill.color;
        bgC.a = 1;
        fillC.a = 1;
        sliderBg.color = bgC;
        sliderFill.color = fillC;
    }
}