using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerText : MonoBehaviour
{
    public int announcerTextSpeed = 1;
    [SerializeField] private GameObject announcerText = null;
    private Vector3 announcerPosition;
    private RectTransform announcerRectTransform;
    private float announcerTextWidth;

    private void Start()
    {
        announcerRectTransform = announcerText.GetComponent<RectTransform>();
    }

    void Update()
    {
        announcerText.transform.Translate(-announcerTextSpeed*Time.deltaTime,0,0);
        announcerPosition = announcerText.transform.position;
        announcerTextWidth = announcerRectTransform.sizeDelta.x;
        if (announcerPosition.x < -announcerTextWidth/2 + 200)
        {
            announcerPosition.x = (announcerTextWidth/2) - 60;
        }
        announcerText.transform.position = announcerPosition;
    }
}
