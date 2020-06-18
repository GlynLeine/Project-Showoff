using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBarScript : MonoBehaviour
{
    public Image pollutionBar;
    public Image natureBar;
    public Image creatureBar;
    void Update()
    {
        pollutionBar.fillAmount = Mathf.Clamp(GameManager.pollution / 3400, 0, 1);
        natureBar.fillAmount = Mathf.Clamp(GameManager.nature / 500, 0, 1);
        creatureBar.fillAmount = Mathf.Clamp(GameManager.happiness / 240, 0, 1);
    }
}
