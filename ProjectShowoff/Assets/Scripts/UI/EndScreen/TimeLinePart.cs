using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLinePart : MonoBehaviour
{
    public enum seasonPart
    {
        spring,
        summer,
        autumn,
        winter
    }
    public seasonPart seasonPartChoice;
    //floats
    private float timeLinePollution;
    private float timeLineWaterLevel;
    private float timeLineBuildings;
    private float timeLineNature;
    private float maxPollution;
    private float maxWaterLevel;
    private float maxBuildings;
    private float maxNature;
    private float buildingCounter;
    private float natureCounter;
    private Color color;
    //gameobjects
    public Image graySky;
    public GameObject waterLevel;
    public GameObject sideWater;
    public GameObject iceCap;
    public GameObject buildings;
    public GameObject nature;

    void OnEnable()
    {
        if (seasonPartChoice == seasonPart.spring)
        {
            timeLinePollution = TimeLineSaver._springPollution;
            timeLineWaterLevel = TimeLineSaver._springWaterLevel;
            timeLineBuildings = TimeLineSaver._springBuildings;
            timeLineNature = TimeLineSaver._springNature;
            maxPollution = 400;
            maxWaterLevel = 1;
            maxBuildings = 12;
            maxNature = 150;
        }
        if (seasonPartChoice == seasonPart.summer)
        {
            timeLinePollution = TimeLineSaver._summerPollution;
            timeLineWaterLevel = TimeLineSaver._summerWaterLevel;
            timeLineBuildings = TimeLineSaver._summerBuildings;
            timeLineNature = TimeLineSaver._summerNature;
            maxPollution = 1000;
            maxWaterLevel = 1;
            maxBuildings = 24;
            maxNature = 300;
        }
        if (seasonPartChoice == seasonPart.autumn)
        {
            timeLinePollution = TimeLineSaver._autumnPollution;
            timeLineWaterLevel = TimeLineSaver._autumnWaterLevel;
            timeLineBuildings = TimeLineSaver._autumnBuildings;
            timeLineNature = TimeLineSaver._autumnNature;
            maxPollution = 1600;
            maxWaterLevel = 1;
            maxBuildings = 36;
            maxNature = 450;
        }
        if (seasonPartChoice == seasonPart.winter)
        {
            timeLinePollution = TimeLineSaver._winterPollution;
            timeLineWaterLevel = TimeLineSaver._winterWaterLevel;
            timeLineBuildings = TimeLineSaver._winterBuildings;
            timeLineNature = TimeLineSaver._winterNature;
            maxPollution = 2300;
            maxWaterLevel = 1;
            maxBuildings = 48;
            maxNature = 600;
        }
        buildingCounter = timeLineBuildings / maxBuildings * 5;
        natureCounter = timeLineNature / maxNature * 8;
        color = graySky.color;
        color.a = timeLinePollution / maxPollution;
        graySky.color = color;
        if (timeLineWaterLevel <= 0.5f)
        {
            Vector3 transformSave = waterLevel.transform.localPosition;
            transformSave.y += 200 * timeLineWaterLevel;
            waterLevel.transform.localPosition = transformSave;
            if (sideWater != null)
            {
                transformSave = sideWater.transform.localPosition;
                transformSave.y += 200 * timeLineWaterLevel;
                sideWater.transform.localPosition = transformSave;
            }
        }else
        {
            Vector3 transformSave = waterLevel.transform.localPosition;
            transformSave.y += 100;
            waterLevel.transform.localPosition = transformSave;
            if (sideWater != null)
            {
                transformSave = sideWater.transform.localPosition;
                transformSave.y += 100;
                sideWater.transform.localPosition = transformSave;
            }
        }
        if (timeLineWaterLevel >= 0.2f)
        {
            iceCap.SetActive(false);
        }
        buildingCounter -= timeLineWaterLevel * 10;
        natureCounter -= timeLineWaterLevel * 16;
        foreach (Transform child in buildings.transform)
        {
            if (buildingCounter < 0)
            {
                Destroy(child.gameObject);
            }
            buildingCounter -= 1;
        }
        foreach (Transform child in nature.transform)
        {
            if (natureCounter < 0)
            {
                Destroy(child.gameObject);
            }
            natureCounter -= 1;
        } 

    }
}
