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
    public int maxAmount;
    private int tickAmount;
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
            yourValue = 2200;
            averageValue = 1500;
        }
        else if (counterType == typeChoice.WaterLevel)
        {
            yourValue = 3100;
            averageValue = 900;
        }
        else if (counterType == typeChoice.Ozone)
        {
            yourValue = GameManager.ozone;
            averageValue = 4500;
        }
        else if (counterType == typeChoice.Plants)
        {
            yourValue = 150;
            averageValue = 225;
        }
        else if (counterType == typeChoice.BuildingsDestroyed)
        {
            yourValue = 5;
            averageValue = 7;
        }
        else if (counterType == typeChoice.BuildingsPlaced)
        {
            yourValue = 50;
            averageValue = 53;
        }
        
        tickAmount = maxAmount / 5;
        for (int i = 0; i < yourValue; i+=tickAmount)
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
        for (int i = 0; i < averageValue; i+=tickAmount)
        {
            if (i <= maxAmount)
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
