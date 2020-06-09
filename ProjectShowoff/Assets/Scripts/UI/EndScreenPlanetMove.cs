using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenPlanetMove : MonoBehaviour
{
    public Zoom zoom;
    public Planet planet;
    public GameObject mainCamera;

    private void OnEnable()
    {
        zoom.enabled = false;
        planet.enabled = false;
        Vector3 cameraPosition = mainCamera.transform.localPosition;
        cameraPosition.Set(6.2f,-0.2f,-9);
        mainCamera.transform.localPosition = cameraPosition;
        planet.enabled = true;
    }
}
