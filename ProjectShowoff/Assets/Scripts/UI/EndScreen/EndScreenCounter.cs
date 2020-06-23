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
        Happiness,
        Plants,
        Industry,
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

        GameState averageState = GameManager.LoadAverages();

        //reflection
        if (counterType == typeChoice.C02)
        {
            yourValue = (float)GameManager.pollution;
            averageValue = averageState.pollution;
        }
        else if (counterType == typeChoice.Happiness)
        {
            yourValue = (float)GameManager.happiness;
            averageValue = averageState.happiness;
        }
        else if (counterType == typeChoice.Plants)
        {
            yourValue = (float)GameManager.nature;
            averageValue = averageState.nature;
        }
        else if (counterType == typeChoice.Industry)
        {
            yourValue = (float)GameManager.industry;
            averageValue = averageState.industry;
        }
        else if (counterType == typeChoice.BuildingsPlaced)
        {
            yourValue = (float)GameManager.buildingsPlaced;
            averageValue = averageState.buildingsPlaced;
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
