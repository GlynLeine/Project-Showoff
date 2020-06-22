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
    void Start()
    {
        timerText = gameObject.GetComponent<TMP_Text>();
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
        timerText.text = minutesLeft + ":" + secondsLeftString;
    }
}
