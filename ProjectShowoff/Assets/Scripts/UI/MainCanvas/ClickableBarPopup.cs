using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableBarPopup : MonoBehaviour
{
    public GameObject ruralBuildings;
    public GameObject coastalBuildings;
    public GameObject destroyBar;
    private float animationSpeed = 1;
    private bool testHasStarted;
    private bool testHasStarted2;

    /*//test to see if animations work
    void Update()
    {
        if (Time.time > 5)
        {
            if (!testHasStarted)
            {
                testHasStarted = true;
                DestroyStart();
            }
        }

        if (Time.time > 10)
        {
            if (!testHasStarted2)
            {
                testHasStarted2 = true;
                CoastalStart();
            }
        }
    }*/
    
    //stop function only has to be called when clicking away, otherwise start functions naturally call the stop functions for other bars
    public void RuralStart()
    {
        StartCoroutine(RuralBuildingAnimationStart());
    }
    public void RuralStop()
    {
        StartCoroutine(RuralBuildingAnimationStop());
    }
    public void CoastalStart()
    {
        StartCoroutine(CoastalBuildingAnimationStart());
    }
    public void CoastalStop()
    {
        StartCoroutine(CoastalBuildingAnimationStop());
    }
    public void DestroyStart()
    {
        StartCoroutine(BuildingOnClickAnimationStart());
    }
    public void DestroyStop()
    {
        StartCoroutine(BuildingOnClickAnimationStop());
    }
    IEnumerator RuralBuildingAnimationStart()
    {
        StartCoroutine(CoastalBuildingAnimationStop());
        StartCoroutine(BuildingOnClickAnimationStop());
        while (ruralBuildings.transform.localPosition.y < -420)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = ruralBuildings.transform.localPosition;
            animationSpeed += 0.1f;
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
    IEnumerator CoastalBuildingAnimationStart()
    {
        StartCoroutine(RuralBuildingAnimationStop());
        StartCoroutine(BuildingOnClickAnimationStop());
        while (coastalBuildings.transform.localPosition.y < -420)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = coastalBuildings.transform.localPosition;
            animationSpeed += 0.1f;
            buildingAnimationPosition.y += 360 * animationSpeed *Time.deltaTime;
            if (buildingAnimationPosition.y >= -420)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -420;
                coastalBuildings.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                coastalBuildings.transform.localPosition = buildingAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator CoastalBuildingAnimationStop()
    {
        StopCoroutine(CoastalBuildingAnimationStart());
        while (coastalBuildings.transform.localPosition.y > -640)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = coastalBuildings.transform.localPosition;
            animationSpeed += 0.1f;
            buildingAnimationPosition.y -= 360 * animationSpeed *Time.deltaTime;
            if (buildingAnimationPosition.y <= -640)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -640;
                coastalBuildings.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                coastalBuildings.transform.localPosition = buildingAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator BuildingOnClickAnimationStart()
    {
        StartCoroutine(RuralBuildingAnimationStop());
        StartCoroutine(CoastalBuildingAnimationStop());
        while (destroyBar.transform.localPosition.y < -420)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = destroyBar.transform.localPosition;
            animationSpeed += 0.1f;
            buildingAnimationPosition.y += 360 * animationSpeed *Time.deltaTime;
            if (buildingAnimationPosition.y >= -420)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -420;
                destroyBar.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                destroyBar.transform.localPosition = buildingAnimationPosition;   
            }
            yield return null;
        }
    }
    IEnumerator BuildingOnClickAnimationStop()
    {
        StopCoroutine(BuildingOnClickAnimationStart());
        while (destroyBar.transform.localPosition.y > -640)
        {
            Vector3 buildingAnimationPosition;
            buildingAnimationPosition = destroyBar.transform.localPosition;
            animationSpeed += 0.1f;
            buildingAnimationPosition.y -= 360 * animationSpeed *Time.deltaTime;
            if (buildingAnimationPosition.y <= -640)
            {
                animationSpeed = 1;
                buildingAnimationPosition.y = -640;
                destroyBar.transform.localPosition = buildingAnimationPosition;  
            }
            else
            {
                destroyBar.transform.localPosition = buildingAnimationPosition;   
            }
            yield return null;
        }
    }
}
