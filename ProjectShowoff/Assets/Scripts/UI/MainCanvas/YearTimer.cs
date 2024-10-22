﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YearTimer : MonoBehaviour
{
    [SerializeField] private Image springGrayscaleImage = null;
    [SerializeField] private Image summerGrayscaleImage = null;
    [SerializeField] private Image fallGrayscaleImage = null;
    [SerializeField] private Image winterGrayscaleImage = null;
    public GameObject endScreenCanvas;
    public GameObject mainCanvas;
    private bool endScreenActivated;
    public Animator endAnimation;
    public Animator endTransition;

    float timeScale = 1f;

    void Update()
    {
        GameManager.SetSeasonTime(Mathf.Clamp01((GameManager.time - 75f / timeScale) / (225f / timeScale)));
        if (GameManager.time <= 75f / timeScale)
        {
            springGrayscaleImage.fillAmount = 1f - GameManager.time / (75f / timeScale);
        }
        else if (GameManager.time <= 150f / timeScale)
        {
            summerGrayscaleImage.fillAmount = 1f - (GameManager.time - (75f / timeScale)) / (75f / timeScale);
        }
        else if (GameManager.time <= 225f / timeScale)
        {
            fallGrayscaleImage.fillAmount = 1f - (GameManager.time - (150f / timeScale)) / (75f / timeScale);
        }
        else if (GameManager.time <= 300f / timeScale)
        {
            winterGrayscaleImage.fillAmount = 1f - (GameManager.time - (225f / timeScale)) / (75f / timeScale);
        }
        else if (!endScreenActivated)
        {
            StartCoroutine(EndTransitionCountdown());
            GameManager.paused = true;
        }
    }

    IEnumerator EndTransitionCountdown()
    {
        endAnimation.Play("Base Layer.timeImages_animation",0);
        yield return new WaitForSeconds(3);
        endTransition.gameObject.SetActive(true);
        endTransition.Play("Base Layer.AnimationCanvas_transitionAnimation",0);
        yield return new WaitForSeconds(6);
        endScreenCanvas.SetActive(true);
        endScreenActivated = true;
        mainCanvas.SetActive(false);
    }
}
