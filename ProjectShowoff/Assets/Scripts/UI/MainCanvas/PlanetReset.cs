﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetReset : MonoBehaviour
{
    [SerializeField] private string loadSceneName = "Planet";
    [SerializeField] private TMP_Text secondsLeft = null;
    [SerializeField] private GameObject resetPanel = null;

    public int secondsTillDestruction = 5;
    
    private float timer = 0;
    private bool resetPressed = false;
    private bool resetTriggered = false;

    private void OnEnable()
    {
        resetTriggered = false;
        resetPressed = false;
        resetPanel.SetActive(false);
    }

    public void SceneResetButton()
    {
        resetPressed = true;
        resetPanel.SetActive(true);
    }

    public void StopResetButton()
    {
        resetPressed = false;
        resetTriggered = false;
        timer = 0;
        resetPanel.SetActive(false);
    }

    public void ResetOnNoInteract()
    {
        resetTriggered = true;
        resetPanel.SetActive(true);
    }
    
    void Update()
    {
        if (resetPressed)
        {
            timer += Time.deltaTime;
            secondsLeft.text = Mathf.CeilToInt(secondsTillDestruction-timer).ToString();
            if (timer > secondsTillDestruction)
            {
                SceneManager.LoadScene(loadSceneName);
            }
        }
        else if (resetTriggered)
        {
            timer += Time.deltaTime;
            secondsLeft.text = Mathf.CeilToInt(10-timer).ToString();
            if (timer > 10)
            {
                SceneManager.LoadScene(loadSceneName);
            }
        }
    }
}
