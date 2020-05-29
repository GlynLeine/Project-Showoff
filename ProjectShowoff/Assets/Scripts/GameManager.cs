using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static float environment;
    public static float pollution;
    public static float industry;
    public Text debugText;
    public Material masterMaterial;
    static Material masterMat;

    public Material ozoneMaterial;
    static Material ozoneMat;

    public Material cloudMaterial;
    static Material cloudMat;

    public static float season;
    public static float ozone;
    public static float cloudiness;
    public static float time;

    public GameObject ocean;

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

    static public void SetCloudState(float cloudState)
    {
        cloudiness = cloudState;
        cloudMat.SetFloat("_Cloudiness", cloudState);
    }

    static public void AddState(float environmentEffect, float pollutionEffect, float industryEffect)
    {
        environment += environmentEffect;
        pollution += pollutionEffect;
        industry += industryEffect;
    }

    private void Awake()
    {
        if (masterMat == null)
            masterMat = masterMaterial;
        if (ozoneMat == null)
            ozoneMat = ozoneMaterial;
        if (cloudMat == null)
            cloudMat = cloudMaterial;
    }

    public static float smoothstep(float min, float max, float interp)
    {
        return Mathf.Clamp01((interp - min) / (max - min));
    }

    public static float lerp(float a, float b, float interp)
    {
        return b * interp + a * (1f - interp);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 1000;
        environment = 50f;
        pollution = 0f;
        industry = 0f;
        season = 0f;
        ozone = 0f;
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        time = t;

        pollution -= (environment / 50f) * (Time.deltaTime / 5f);

        float uniformScale = 1f + smoothstep(200f, 3800f, pollution) * 0.07f;
        ocean.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);

        SetOzoneState(smoothstep(100f, 2000f, pollution) * 0.7f);

        SetCloudState(smoothstep(-1000f, 3000f, pollution) * 0.6f);

        if (debugText != null)
        {
            //float spring = smoothstep(1f / 3f, 0f, season);
            //float summer = smoothstep(0f, 1f / 3f, season) * smoothstep(2f / 3f, 1f / 3f, season);
            //float fall = smoothstep(1f / 3f, 2f / 3f, season) * smoothstep(1f, 2f / 3f, season);
            //float winter = smoothstep(2f / 3f, 1f, season);

            //debugText.text = "environment: " + environment;
            debugText.text = "\npollution: " + pollution;
            //debugText.text += "\nindustry: " + industry;
            debugText.text += "\nfps: " + 1f / Time.deltaTime;
            debugText.text += "\nframetime: " + Time.deltaTime;
            debugText.text += "\ngraphics device: " + SystemInfo.graphicsDeviceType.ToString();
            //debugText.text += "\ntime: " + time;
            //debugText.text += "\nspring: " + spring;
            //debugText.text += "\nsummer: " + summer;
            //debugText.text += "\nfall: " + fall;
            //debugText.text += "\nwinter: " + winter;
            //debugText.text += "\nseason: " + (spring + summer > fall + winter ? (spring > summer ? "spring" : "summer") : (fall > winter ? "fall" : "winter")) + " " + season;
            //debugText.text += "\nozone: " + ozone;
        }
    }
}
