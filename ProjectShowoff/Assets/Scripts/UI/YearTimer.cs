using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YearTimer : MonoBehaviour
{
    private float timer = 0;
    [SerializeField] private Image spring1GrayscaleImage = null;
    [SerializeField] private Image summerGrayscaleImage = null;
    [SerializeField] private Image fallGrayscaleImage = null;
    [SerializeField] private Image winterGrayscaleImage = null;
    [SerializeField] private Image spring2GrayscaleImage = null;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer <= 60)
        {
            spring1GrayscaleImage.fillAmount = 1 - timer / 60;
        }
        else if (timer > 60 && timer <= 120)
        {
            summerGrayscaleImage.fillAmount = 1 - (timer-60) / 60;
        }
        else if (timer > 120 && timer <= 180)
        {
            fallGrayscaleImage.fillAmount = 1 - (timer-120) / 60;
        }
        else if (timer > 180 && timer <= 240)
        {
            winterGrayscaleImage.fillAmount = 1 - (timer-180) / 60;
        }
        else if (timer > 240 && timer <= 300)
        {
            spring2GrayscaleImage.fillAmount = 1 - (timer-240) / 60;
        }
    }
}
