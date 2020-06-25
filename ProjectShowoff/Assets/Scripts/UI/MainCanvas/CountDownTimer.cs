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
    private bool flashingHappened;
    private float flashTimer;
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
        if (minutesLeft < 1 && secondsLeft < 31)
        {
            if (!flashingHappened)
            {
                StartCoroutine(CountDownAnimation());
                flashingHappened = true;
            }
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

    private IEnumerator CountDownAnimation()
    {
        Color timerColor;
        flashTimer = 0;
        while (flashTimer < 0.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 1)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 1.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 2)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 2.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 3)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 3.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 4)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 4.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 5)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 5.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 6)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 6.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 7)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 7.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 8)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 8.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 9)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 9.5)
        {
            timerColor = timerText.color;
            timerColor.b -= Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g -= Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        while (flashTimer < 10)
        {
            timerColor = timerText.color;
            timerColor.b += Mathf.Clamp01(2*Time.deltaTime);
            timerColor.g += Mathf.Clamp01(2*Time.deltaTime);
            timerText.color = timerColor;
            flashTimer += Time.deltaTime;
            yield return null;
        }
        timerText.color = Color.white;
        flashingHappened = false;
    }
}
