using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YearTimer : MonoBehaviour
{
    [SerializeField] private Image spring1GrayscaleImage = null;
    [SerializeField] private Image summerGrayscaleImage = null;
    [SerializeField] private Image fallGrayscaleImage = null;
    [SerializeField] private Image winterGrayscaleImage = null;
    [SerializeField] private Image spring2GrayscaleImage = null;

    //void Update()
    //{
    //    GameManager.SetSeasonTime(Mathf.Clamp01((GameManager.time - 3f) / 24f));
    //    if (GameManager.time <= 6)
    //    {
    //        spring1GrayscaleImage.fillAmount = 1f - GameManager.time / 6f;
    //    }
    //    else if (GameManager.time <= 12)
    //    {
    //        summerGrayscaleImage.fillAmount = 1f - (GameManager.time - 6f) / 6f;
    //    }
    //    else if (GameManager.time <= 18)
    //    {
    //        fallGrayscaleImage.fillAmount = 1f - (GameManager.time - 12f) / 6f;
    //    }
    //    else if (GameManager.time <= 24)
    //    {
    //        winterGrayscaleImage.fillAmount = 1f - (GameManager.time - 18f) / 6f;
    //    }
    //    else if (GameManager.time <= 30)
    //    {
    //        spring2GrayscaleImage.fillAmount = 1f - (GameManager.time - 24f) / 6f;
    //    }
    //}

    void Update()
    {
        GameManager.SetSeasonTime(Mathf.Clamp01((GameManager.time - 30f) / 240f));
        if (GameManager.time <= 60)
        {
            spring1GrayscaleImage.fillAmount = 1f - GameManager.time / 60f;
        }
        else if (GameManager.time <= 120)
        {
            summerGrayscaleImage.fillAmount = 1f - (GameManager.time - 60f) / 60f;
        }
        else if (GameManager.time <= 180)
        {
            fallGrayscaleImage.fillAmount = 1f - (GameManager.time - 120f) / 60f;
        }
        else if (GameManager.time <= 240)
        {
            winterGrayscaleImage.fillAmount = 1f - (GameManager.time - 180f) / 60f;
        }
        else if (GameManager.time <= 300)
        {
            spring2GrayscaleImage.fillAmount = 1f - (GameManager.time - 240f) / 60f;
        }
    }
}
