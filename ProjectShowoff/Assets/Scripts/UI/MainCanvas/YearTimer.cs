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
    public GameObject endScreenCanvas;
    public GameObject mainCanvas;
    private bool endScreenActivated;

    float timeScale = 1f;

    void Update()
    {
        GameManager.SetSeasonTime(Mathf.Clamp01((GameManager.time - 30f / timeScale) / (180f / timeScale)));
        if (GameManager.time <= 60f / timeScale)
        {
            springGrayscaleImage.fillAmount = 1f - GameManager.time / (60f / timeScale);
        }
        else if (GameManager.time <= 120f / timeScale)
        {
            summerGrayscaleImage.fillAmount = 1f - (GameManager.time - (60f / timeScale)) / (60f / timeScale);
        }
        else if (GameManager.time <= 180f / timeScale)
        {
            fallGrayscaleImage.fillAmount = 1f - (GameManager.time - (120f / timeScale)) / (60f / timeScale);
        }
        else if (GameManager.time <= 240f / timeScale)
        {
            winterGrayscaleImage.fillAmount = 1f - (GameManager.time - (180f / timeScale)) / (60f / timeScale);
        }
        else if (!endScreenActivated)
        {
            endScreenCanvas.SetActive(true);
            endScreenActivated = true;
            GameManager.paused = true;
            mainCanvas.SetActive(false);
        }
    }
}
