using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenCounter : MonoBehaviour
{
    // objects
    public GameObject filledPrefab;
    public GameObject halfPrefab;
    public GameObject yourFill;
    public GameObject theirFill;

    public enum typeChoice
    {
        C02,
        WaterLevel,
        Ozone,
        Plants,
        BuildingsDestroyed,
        BuildingsPlaced
    };
    public float maxAmount;
    private float tickAmount;
    private float yourValue = 2000;
    private float averageValue = 2000;
    public typeChoice counterType;
    private void OnEnable()
    {
        foreach (Transform child in yourFill.transform) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in theirFill.transform) {
            Destroy(child.gameObject);
        }
        //reflection
        if (counterType == typeChoice.C02)
        {
            yourValue = (float)GameManager.pollution;
            averageValue = 1500f;
        }
        else if (counterType == typeChoice.WaterLevel)
        {
            yourValue = (float)GameManager.waterLevel;
            averageValue = 0.5f;
        }
        else if (counterType == typeChoice.Ozone)
        {
            yourValue = (float)GameManager.ozone;
            averageValue = 0.5f;
        }
        else if (counterType == typeChoice.Plants)
        {
            yourValue = (float)GameManager.nature;
            averageValue = 225f;
        }
        else if (counterType == typeChoice.BuildingsDestroyed)
        {
            yourValue = (float)GameManager.buildingsDestroyed;
            averageValue = 7f;
        }
        else if (counterType == typeChoice.BuildingsPlaced)
        {
            yourValue = (float)GameManager.buildingsPlaced;
            averageValue = 53f;
        }
        tickAmount = maxAmount / 5;
        for (float i = 0; i < yourValue; i+=tickAmount)
        {
            if (i < maxAmount)
            {
                if (yourValue - i >= tickAmount)
                {
                    Instantiate(filledPrefab, yourFill.transform);
                }
                else if (yourValue - i >= tickAmount/2)
                {
                    Instantiate(halfPrefab, yourFill.transform);
                }
            }
        }
        for (float i = 0; i < averageValue; i+=tickAmount)
        {
            if (i < maxAmount)
            {
                if (averageValue - i >= tickAmount)
                {
                    Instantiate(filledPrefab, theirFill.transform);
                }
                else if (averageValue - i >= tickAmount/2)
                {
                    Instantiate(halfPrefab, theirFill.transform);
                }   
            }
        }
    }
}
