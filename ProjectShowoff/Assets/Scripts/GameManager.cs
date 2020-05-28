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

    float smoothstep(float min, float max, float interp)
    {
        return Mathf.Clamp01((interp - min) / (max - min));
    }

    float lerp(float a, float b, float interp)
    {
        return b * interp + a * (1f - interp);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        time = t;

        if (debugText != null)
        {
            float spring = lerp(smoothstep(0.25f, 0f, season), smoothstep(0.75f, 1f, season), smoothstep(0.25f, 0.5f, season));
            float summer = smoothstep(0f, 0.25f, season) * smoothstep(0.5f, 0.25f, season);
            float fall = smoothstep(0.25f, 0.5f, season) * smoothstep(0.75f, 0.5f, season);
            float winter = smoothstep(0.5f, 0.75f, season) * smoothstep(1f, 0.75f, season);


            debugText.text = "environment: " + environment;
            debugText.text += "\npollution: " + pollution;
            debugText.text += "\nhappiness: " + happiness;
            debugText.text += "\nfps: " + 1f / Time.deltaTime;
            debugText.text += "\nframetime: " + Time.deltaTime;
            debugText.text += "\ntime: " + time;
            debugText.text += "\nspring: " + spring;
            debugText.text += "\nsummer: " + summer;
            debugText.text += "\nfall: " + fall;
            debugText.text += "\nwinter: " + winter;
            debugText.text += "\nseason: " + (spring + summer > fall + winter ? (spring > summer ? "spring" : "summer") : (fall > winter ? "fall" : "winter")) + " " + season;
            debugText.text += "\nozone: " + ozone;
        }
    }
}
