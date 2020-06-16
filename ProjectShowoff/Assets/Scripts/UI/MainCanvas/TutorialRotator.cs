using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRotator : MonoBehaviour
{
    public Transform cameraRotationObject;
    public Zoom zoomScript;
    public BuildingSystem buildingSystem;
    public float rotationDuration = 1f;
    public float endZoom = 2f;

    void Start()
    {
        StartCoroutine(zoomDone());
    }

    IEnumerator zoomDone()
    {
        float timeBuffer = 0;

        Vector3 toStartLocation = (buildingSystem.startLocation.transform.position - cameraRotationObject.position).normalized;

        Quaternion startRotation = cameraRotationObject.rotation;
        Quaternion endRotation = Quaternion.LookRotation(-toStartLocation, cameraRotationObject.up);

        float startZoom = zoomScript.range;

        while (timeBuffer < rotationDuration)
        {
            timeBuffer += Time.deltaTime;
            float interp01 = GameManager.smoothstep(0, rotationDuration, timeBuffer);

            zoomScript.range = GameManager.lerp(startZoom, endZoom, interp01);

            cameraRotationObject.rotation = Quaternion.Lerp(startRotation, endRotation, interp01);

            yield return null;
        }

        zoomScript.range = endZoom;
        cameraRotationObject.rotation = endRotation;
    }
}
