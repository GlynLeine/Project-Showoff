using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public int totalTimeInSeconds = 300;
    private TMP_Text timerText;
    private int minutesLeft;
    private int secondsLeft;
    private string secondsLeftString;
    private float timer;
    private bool english;
    void Start()
    {
        timerText = gameObject.GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
        {
            english = true;
        }
        else
        {
            english = false;
        }
    }

    void Update()
    {
        timer += GameManager.deltaTime;
        minutesLeft = Mathf.FloorToInt((totalTimeInSeconds - timer) / 60);
        secondsLeft = Mathf.FloorToInt((totalTimeInSeconds - 60*minutesLeft) - timer);
        if (secondsLeft < 10)
        {
            secondsLeftString = "0" + secondsLeft;
        }
        else
        {
            secondsLeftString = secondsLeft.ToString();
        }

        if (GameManager.paused)
        {
            if (LanguageSelector.LanguageSelected == LanguageSelector.LanguageSelectorSelected.English)
            {
                if (english)
                {
                    timerText.text = "PAUSED";
                }
                else
                {
                    timerText.text = "GEPAUZEERD";
                }
            }
        }
        else
        {
            timerText.text = minutesLeft + ":" + secondsLeftString;
        }
    }
}
