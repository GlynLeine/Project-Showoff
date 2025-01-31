﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class GameState
{
    public float nature;
    public float pollution;
    public float industry;
    public float happiness;
    public float waterLevel;
    public float buildingsPlaced;
    public ulong userCount;
}

public class GameManager : MonoBehaviour
{
    public static float nature;
    public static float pollution;
    public static float industry;
    public static float happiness;

    public static int buildingsPlaced;
    public static int buildingsDestroyed;
    public static float waterLevel;
    public static int continentsDiscovered;
    public static bool airPlanes;
    public static bool spaceShips;
    public static bool satellites;
    public static float maxPollution;
    public static float minPollution;
    public static float deltaPollution;
    public static int natureBuildings;
    public static int ruralBuildings;
    public static int coastalBuildings;
    public static int buildingsFlooded;
    public static int maxBuildings;
    public static int creaturesPoked;

    public static float coolDown;

    public Text debugText;
    public Material masterMaterial;
    static Material masterMat;

    public Material treeMaterial;
    static Material treeMat;

    public Shader tesselationShader;
    public Shader webGLShader;

    public Material ozoneMaterial;
    static Material ozoneMat;

    public Material cloudMaterial;
    static Material cloudMat;

    public static float season;
    public static float ozone;
    public static float iceCaps;
    public static float climate;
    public static float cloudiness;
    public static float time;
    public static float deltaTime;
    public static bool paused;

    public bool skipCooldown;
    [Range(1f, 10f)]
    public float timeScale = 1f;

    public GameObject ocean;

    private float t;
    private float prevPollut = 0;


    static float AppendAverage(float average, float newData, ulong count)
    {
        if (count == 0)
            return newData;

        decimal contribution = 1m / count;
        decimal newAverage = (decimal)newData * contribution;
        newAverage /= 1m + contribution;
        return (float)newAverage;
    }

    static public void AppendDataAndOverwrite()
    {
        GameState averageState = LoadAverages();

        averageState.nature = AppendAverage(averageState.nature, nature, averageState.userCount);
        averageState.pollution = AppendAverage(averageState.pollution, pollution, averageState.userCount);
        averageState.industry = AppendAverage(averageState.industry, industry, averageState.userCount);
        averageState.happiness = AppendAverage(averageState.happiness, happiness, averageState.userCount);
        averageState.waterLevel = AppendAverage(averageState.waterLevel, waterLevel, averageState.userCount);
        averageState.buildingsPlaced = AppendAverage(averageState.buildingsPlaced, buildingsPlaced, averageState.userCount);

        if (averageState.userCount < ulong.MaxValue)
            averageState.userCount++;

        string path = Application.persistentDataPath + "/Data/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        FileStream file = File.Create(path + "gamestate.data");

        try
        {
            new BinaryFormatter().Serialize(file, averageState);
        }
        catch (SerializationException e)
        {
            Debug.LogWarning(e.Message);
        }
        finally
        {
            file.Close();
        }

    }

    private static GameState averageStateCache;

    static public GameState LoadAverages()
    {
        if (averageStateCache != null)
            return averageStateCache;

        string path = Application.persistentDataPath + "/Data/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        path += "gamestate.data";

        GameState averageState = new GameState();

        if (File.Exists(path))
        {
            Debug.Log("Loading file: " + path);
            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                averageState = (GameState)new BinaryFormatter().Deserialize(file);
                averageStateCache = averageState;

                if (averageState.userCount == 0)
                {
                    averageState.userCount++;
                    averageState.nature = 250f;
                    averageState.pollution = 780f;
                    averageState.industry = 21f;
                    averageState.happiness = 80f;
                    averageState.waterLevel = 0.17f;
                    averageState.buildingsPlaced = 47f;
                }

                Debug.Log("average nature" + averageState.nature);
                Debug.Log("average pollution" + averageState.pollution);
                Debug.Log("average industry" + averageState.industry);
                Debug.Log("average happiness" + averageState.happiness);
                Debug.Log("average waterLevel" + averageState.waterLevel);
                Debug.Log("average buildingsPlaced" + averageState.buildingsPlaced);
                Debug.Log("average userCount" + averageState.userCount);

                Debug.Log("Loaded file: " + path);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            finally
            {
                file.Close();
            }
        }
        else
        {
            Debug.LogWarning("File not found");
        }

        return averageState;
    }

    static public void SetSeasonTime(float seasonTime)
    {
        season = seasonTime;
        masterMat.SetFloat("_SeasonTime", seasonTime);
        treeMat.SetFloat("_SeasonTime", seasonTime);
    }

    static public void SetOzoneState(float ozoneState)
    {
        ozone = ozoneState;
        ozoneMat.SetFloat("_Dissolve", lerp(0.25f, 0.7f, ozoneState));
    }

    static public void SetCloudState(float cloudState)
    {
        cloudiness = cloudState;
        cloudMat.SetFloat("_Cloudiness", cloudState * 0.6f);
    }

    static public void SetClimateState(float climateState)
    {
        climate = climateState;
        masterMat.SetFloat("_Pollution", climateState);
        treeMat.SetFloat("_Pollution", climateState);
    }

    static public void AddState(float natureEffect, float pollutionEffect, float industryEffect, float happinessEffect)
    {
        nature += natureEffect;
        pollution += pollutionEffect;
        industry += industryEffect;
        happiness += happinessEffect;
    }

    private void Awake()
    {
        if (masterMat == null)
            masterMat = masterMaterial;
        if (treeMat == null)
            treeMat = treeMaterial;
        if (ozoneMat == null)
            ozoneMat = ozoneMaterial;
        if (cloudMat == null)
            cloudMat = cloudMaterial;

        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
            masterMat.shader = webGLShader;
        else
            masterMat.shader = tesselationShader;
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
        averageStateCache = null;

        SetCloudState(0.25f);
        SetOzoneState(0f);
        SetSeasonTime(0f);


        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 1000;
        nature = 50f;
        pollution = 0f;
        industry = 0f;
        happiness = 0f;

        buildingsPlaced = 0;
        buildingsDestroyed = 0;
        waterLevel = 0f;
        continentsDiscovered = 0;
        airPlanes = false;
        spaceShips = false;
        satellites = false;
        maxPollution = 0f;
        minPollution = 0f;
        deltaPollution = 0f;
        natureBuildings = 0;
        ruralBuildings = 0;
        coastalBuildings = 0;
        buildingsFlooded = 0;
        maxBuildings = 0;
        creaturesPoked = 0;
        if (skipCooldown)
            coolDown = 0f;
        else
            coolDown = 3f;

        t = 0;
        prevPollut = 0;

        iceCaps = 1f;
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = 0;
        if (!paused)
        {
            deltaTime = Time.deltaTime * timeScale;
            t += deltaTime;
            time = t;

            if (pollution > maxPollution)
                maxPollution = pollution;

            pollution = Mathf.Clamp(pollution - (nature / 50f) * (deltaTime / 5f) * 4f, 0, float.MaxValue);

            happiness = Mathf.Clamp(happiness - (0.05f * (pollution / 100f) * (deltaTime / 5f)), 0, float.MaxValue);

            if (pollution < minPollution)
                minPollution = pollution;

            deltaPollution = pollution - prevPollut;
            prevPollut = pollution;

            waterLevel = smoothstep(200f, 3800f, pollution);
            float uniformScale = 1f + waterLevel * 0.07f;
            ocean.transform.localScale = new Vector3(uniformScale, uniformScale, uniformScale);

            iceCaps = smoothstep(3800f, 200f, pollution);

            SetClimateState(smoothstep(100f, 3000f, pollution));

            float linearScale = smoothstep(0, 2000f, pollution);
            SetOzoneState(1f - Mathf.Pow(1f - linearScale, 2f));

            SetCloudState(smoothstep(-1000f, 3000f, pollution));
        }

        if (debugText != null)
        {
            //float spring = smoothstep(1f / 3f, 0f, season);
            //float summer = smoothstep(0f, 1f / 3f, season) * smoothstep(2f / 3f, 1f / 3f, season);
            //float fall = smoothstep(1f / 3f, 2f / 3f, season) * smoothstep(1f, 2f / 3f, season);
            //float winter = smoothstep(2f / 3f, 1f, season);

            debugText.text = "nature: " + nature;
            debugText.text += "\npollution: " + pollution;
            debugText.text += "\nindustry: " + industry;
            debugText.text += "\nhappiness: " + happiness;
            debugText.text += "\nfps: " + 1f / Time.deltaTime;
            debugText.text += "\nframetime: " + Time.deltaTime;
            debugText.text += "\ngraphics device: " + SystemInfo.graphicsDeviceType.ToString();
            debugText.text += "\nshader: " + masterMat.shader.name;
            debugText.text += "\ntime: " + time;
            debugText.text += "\nbuildingsPlaced: " + buildingsPlaced;
            debugText.text += "\nbuildingsDestroyed: " + buildingsDestroyed;
            debugText.text += "\nwaterLevel: " + waterLevel;
            debugText.text += "\ncontinentsDiscovered " + continentsDiscovered;
            debugText.text += "\nairPlanes: " + airPlanes;
            debugText.text += "\nspaceShips: " + spaceShips;
            debugText.text += "\nsatellites: " + satellites;
            debugText.text += "\nmaxPollution: " + maxPollution;
            debugText.text += "\nminPollution: " + minPollution;
            debugText.text += "\ndeltaPollution: " + deltaPollution;
            debugText.text += "\nnatureBuildings: " + natureBuildings;
            debugText.text += "\nruralBuildings: " + ruralBuildings;
            debugText.text += "\ncoastalBuildings: " + coastalBuildings;
            debugText.text += "\nbuildingsFlooded: " + buildingsFlooded;
            debugText.text += "\nmaxBuildings: " + maxBuildings;
            debugText.text += "\ncreaturesPoked: " + creaturesPoked;
            debugText.text += "\nozone: " + ozone;
            debugText.text += "\nclimate: " + climate;
        }
    }
}
