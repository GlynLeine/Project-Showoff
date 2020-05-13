using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static float environment;
    static float pollution;
    static float happiness;
    public Text debugText;

    static public void AddState(float environmentEffect, float pollutionEffect, float happinessEffect)
    {
        environment += environmentEffect;
        pollution += pollutionEffect;
        happiness += happinessEffect;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugText != null)
        {
            debugText.text = "environment: " + environment;
            debugText.text += "\npollution: " + pollution;
            debugText.text += "\nhappiness: " + happiness;
        }
    }
}
