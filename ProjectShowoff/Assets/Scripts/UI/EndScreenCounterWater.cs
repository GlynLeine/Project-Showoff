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
        rectTransform = yourFill.GetComponent<RectTransform>();
        Vector3 transform = rectTransform.anchoredPosition;
        transform.y += 50 * yourValue;
        if (transform.y > 0)
        {
            transform.y = 50;
        }
        rectTransform.anchoredPosition = transform;
        rectTransform = theirFill.GetComponent<RectTransform>();
        transform = rectTransform.anchoredPosition;
        transform.y += 50 * averageValue;
        if (transform.y > 0)
        {
            transform.y = 50;
        }
        rectTransform.anchoredPosition = transform;
    }
}
