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
    public Material masterMaterial;
    static Material masterMat;

    public Material ozoneMaterial;
    static Material ozoneMat;

    public static float season;
    public static float ozone;
    public static float time;

    private float t;

    static public void SetSeasonTime(float seasonTime)
    {
        season = seasonTime;
        masterMat.SetFloat("_SeasonTime", seasonTime);
    }

    static public void SetOzoneState(float ozoneState)
    {
        ozone = ozoneState;
        ozoneMat.SetFloat("_Dissolve", ozoneState);
    }

    static public void AddState(float environmentEffect, float pollutionEffect, float happinessEffect)
    {
        environment += environmentEffect;
        pollution += pollutionEffect;
        happiness += happinessEffect;
    }

    private void Awake()
    {
        if (masterMat == null)
            masterMat = masterMaterial;
        if (ozoneMat == null)
            ozoneMat = ozoneMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        time = t;

        SetSeasonTime(t / 30f);
        SetOzoneState(t / 30f);

        if (debugText != null)
        {
            debugText.text = "environment: " + environment;
            debugText.text += "\npollution: " + pollution;
            debugText.text += "\nhappiness: " + happiness;
            debugText.text += "\nfps: " + 1f / Time.deltaTime;
            debugText.text += "\nframetime: " + Time.deltaTime;
            debugText.text += "\ntime: " + time;
            debugText.text += "\nseason: " + season;
            debugText.text += "\nozone: " + ozone;
        }
    }
}
