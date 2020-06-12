using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenCounterWater : MonoBehaviour
{
    private float yourValue = 0.1f;
    private float averageValue = 0.3f;
    public GameObject yourFill;
    public GameObject theirFill;
    private RectTransform rectTransform;
    private void OnEnable()
    {
        yourValue = (float)GameManager.waterLevel;
        averageValue = 0.5f;
        rectTransform = (RectTransform)yourFill.transform;
        Vector3 waterTransform = rectTransform.anchoredPosition;
        waterTransform.y += 50 * yourValue;
        if (waterTransform.y > 0)
        {
            waterTransform.y = 0;
        }
        rectTransform.anchoredPosition = waterTransform;
        
        rectTransform = (RectTransform)theirFill.transform;
        waterTransform = rectTransform.anchoredPosition;
        waterTransform.y += 50 * averageValue;
        if (waterTransform.y > 0)
        {
            waterTransform.y = 0;
        }
        rectTransform.anchoredPosition = waterTransform;
    }
}
