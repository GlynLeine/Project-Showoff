using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableBarPopup : MonoBehaviour
{
    public GameObject ruralBuildings;
    public GameObject coastalBuildings;
    private float animationSpeed = 1;
    private bool hasStarted;
    private bool hasStarted2;

    //test to see if animations work
    /*void Update()
    {
        if (Time.time > 5)
        {
            if (!hasStarted)
            {
                hasStarted = true;
                RuralStart();
            }
        }

        if (Time.time > 10)
        {
            if (!hasStarted2)
            {
                hasStarted2 = true;
                RuralStop();
            }
        }
    }*/
    
    public void RuralStart()
    {
        StartCoroutine(RuralBuildingAnimationStart());
    }

    public void RuralStop()
    {
        StartCoroutine(RuralBuildingAnimationStop());
    }
    IEnumerator RuralBuildingAnimationStart()
    {
        while (ruralBuildings.transform.localPosition.y < -420)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = ruralBuildings.transform.localPosition;
            animationSpeed += 0.1f;
            Debug.Log(ruralBuildings.transform.localPosition);
            buildingAnimationPosition.y += 360 * animationSpeed *Time.deltaTime;
            if (buildingAnimationPosition.y >= -420)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -420;
                ruralBuildings.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                ruralBuildings.transform.localPosition = buildingAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator RuralBuildingAnimationStop()
    {
        StopCoroutine(RuralBuildingAnimationStart());
        while (ruralBuildings.transform.localPosition.y > -640)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = ruralBuildings.transform.localPosition;
            animationSpeed += 0.1f;
            Debug.Log(ruralBuildings.transform.localPosition);
            buildingAnimationPosition.y -= 360 * animationSpeed *Time.deltaTime;
            if (buildingAnimationPosition.y <= -640)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -640;
                ruralBuildings.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                ruralBuildings.transform.localPosition = buildingAnimationPosition;   
            }
            yield return null;
        }
    }
}
