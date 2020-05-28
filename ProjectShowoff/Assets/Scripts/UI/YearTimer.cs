using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YearTimer : MonoBehaviour
{
    [SerializeField] private Image springGrayscaleImage = null;
    [SerializeField] private Image summerGrayscaleImage = null;
    [SerializeField] private Image fallGrayscaleImage = null;
    [SerializeField] private Image winterGrayscaleImage = null;

    float timeScale = 10f;

    void Update()
    {
        GameManager.SetSeasonTime(Mathf.Clamp01((GameManager.time - 37.5f / timeScale) / (225f / timeScale)));
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
    }
}
