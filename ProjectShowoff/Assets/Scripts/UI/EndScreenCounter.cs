using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenCounter : MonoBehaviour
{
    //c02 objects
    public GameObject filledC02Prefab;
    public GameObject halfC02Prefab;
    public GameObject yourC02;
    public GameObject theirC02;
    //
    public GameObject filledWaterPrefab;
    public GameObject halfWaterPrefab;
    public GameObject yourWater;
    public GameObject theirWater;

    private float waterLevel = 3000;
    private float averageWaterLevel = 3200;
    private float pollution = 1800;
    private float averagePollution = 999;

    private void OnEnable()
    {
        for (int i = 0; i < pollution; i+=500)
        {
            if (pollution - i >= 500)
            {
                Instantiate(filledC02Prefab, yourC02.transform);
            }
            else if (pollution - i >= 250)
            {
                Instantiate(halfC02Prefab, yourC02.transform);
            }
            
        }
        for (int i = 0; i < averagePollution; i+=500)
        {
            if (averagePollution - i >= 500)
            {
                Instantiate(filledC02Prefab, theirC02.transform);
            }
            else if (averagePollution - i >= 250)
            {
                Instantiate(halfC02Prefab, theirC02.transform);
            }
        }
        for (int i = 0; i < waterLevel; i+=800)
        {
            if (waterLevel - i >= 800)
            {
                Instantiate(filledWaterPrefab, yourWater.transform);
            }
            else if (waterLevel - i >= 400)
            {
                Instantiate(halfWaterPrefab, yourWater.transform);
            }
            
        }
        for (int i = 0; i < averageWaterLevel; i+=800)
        {
            if (averageWaterLevel - i >= 800)
            {
                Instantiate(filledWaterPrefab, theirWater.transform);
            }
            else if (averageWaterLevel - i >= 400)
            {
                Instantiate(halfWaterPrefab, theirWater.transform);
            }
        }
    }
}
