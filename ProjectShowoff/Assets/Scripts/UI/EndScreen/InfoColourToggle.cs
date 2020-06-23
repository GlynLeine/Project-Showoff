using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoColourToggle : MonoBehaviour
{
    private bool switchBool;
    private Image buttonColor;
    private Button buttonButton;

    private void Start()
    {
        buttonColor = gameObject.GetComponent<Image>();
        buttonButton = gameObject.GetComponent<Button>();
    }

    public void ColourActiveToggle()
    {
        switchBool = !switchBool;
        if (switchBool)
        {
            buttonColor.color = buttonButton.colors.pressedColor;
        }
        else
        {
            buttonColor.color = buttonButton.colors.normalColor;
        }
    }
}
