using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBarScript : MonoBehaviour
{
    public Image pollutionBar;
    public Image natureBar;
    public Image creatureBar;
    public Image industryBar;
    void Update()
    {
        pollutionBar.fillAmount = Mathf.Clamp(GameManager.pollution / 2000, 0, 1);
        natureBar.fillAmount = Mathf.Clamp(GameManager.nature / 900, 0, 1);
        creatureBar.fillAmount = Mathf.Clamp(GameManager.happiness / 240, 0, 1);
        industryBar.fillAmount = Mathf.Clamp(GameManager.industry/50, 0, 1);
    }
}
