using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableBarPopup : MonoBehaviour
{
    public GameObject ruralBuildings;
    public GameObject coastalBuildings;
    public GameObject destroyBar;
    public GameObject harbour;
    public StatDisplay statDisplay;
    public BuildingSystem buildingSystem;
    private float animationSpeed = 1;
    private bool testHasStarted;
    private bool testHasStarted2;
    private TutorialScript tutorialScript;

    private bool ruralStartActive;
    private bool ruralStopActive;
    private bool coastalStartActive;
    private bool coastalStopActive;
    private bool destroyStartActive;
    private bool destroyStopActive;

    private void OnEnable()
    {
        tutorialScript = GetComponent<TutorialScript>();
    }
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
        if (buildingSystem.selectedLocation.Harbor == null)
            harbour.SetActive(false);
        else if(tutorialScript.tutorialBuildStep)
            harbour.SetActive(true);

        StartCoroutine(CoastalBuildingAnimationStart());
    }
    public void CoastalStop()
    {
        StartCoroutine(CoastalBuildingAnimationStop());
    }
    public void DestroyStart(Building building)
    {
        if (building.effectPeriod > 0)
        {
            statDisplay.SetHappiness(building.industryEffect / building.effectPeriod);
            statDisplay.SetPollution(building.pollutionEffect - building.natureRemovalEffect / building.effectPeriod);
        }
        else
        {
            statDisplay.SetHappiness(0);
            statDisplay.SetPollution(0);
        }
        StartCoroutine(BuildingOnClickAnimationStart());
    }
    public void DestroyStop()
    {
        StartCoroutine(BuildingOnClickAnimationStop());
    }
    IEnumerator RuralBuildingAnimationStart()
    {
        if (!ruralStartActive)
        {
            ruralStartActive = true;
            StartCoroutine(CoastalBuildingAnimationStop());
            StartCoroutine(BuildingOnClickAnimationStop());
            StopCoroutine(RuralBuildingAnimationStop());
            while (ruralBuildings.transform.localPosition.y < -420)
            {

                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = ruralBuildings.transform.localPosition;
                animationSpeed += 0.1f;
                buildingAnimationPosition.y += 360 * animationSpeed * Time.deltaTime;
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

                if (ruralStopActive)
                {
                    break;
                }
                yield return null;
            }
            ruralStartActive = false;
        }
    }
    IEnumerator RuralBuildingAnimationStop()
    {
        if (!ruralStopActive)
        {
            ruralStopActive = true;
            StopCoroutine(RuralBuildingAnimationStart());
            while (ruralBuildings.transform.localPosition.y > -640)
            {
                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = ruralBuildings.transform.localPosition;
                animationSpeed += 0.1f;
                buildingAnimationPosition.y -= 360 * animationSpeed * Time.deltaTime;
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

            ruralStopActive = false;
        }
    }
    IEnumerator CoastalBuildingAnimationStart()
    {
        if (!coastalStartActive)
        {
            coastalStartActive = true;
            StartCoroutine(RuralBuildingAnimationStop());
            StartCoroutine(BuildingOnClickAnimationStop());
            StopCoroutine(CoastalBuildingAnimationStop());
            while (coastalBuildings.transform.localPosition.y < -420)
            {
                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = coastalBuildings.transform.localPosition;
                animationSpeed += 0.1f;
                buildingAnimationPosition.y += 360 * animationSpeed * Time.deltaTime;
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
                if (coastalStopActive)
                {
                    break;
                }
                yield return null;
            }
            coastalStartActive = false;
        }
    }
    IEnumerator CoastalBuildingAnimationStop()
    {
        if (!coastalStopActive)
        {
            coastalStopActive = true;
            StopCoroutine(CoastalBuildingAnimationStart());
            while (coastalBuildings.transform.localPosition.y > -640)
            {
                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = coastalBuildings.transform.localPosition;
                animationSpeed += 0.1f;
                buildingAnimationPosition.y -= 360 * animationSpeed * Time.deltaTime;
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
            coastalStopActive = false;
        }
    }
    IEnumerator BuildingOnClickAnimationStart()
    {
        if (!destroyStartActive)
        {
            destroyStartActive = true;
            StartCoroutine(RuralBuildingAnimationStop());
            StartCoroutine(CoastalBuildingAnimationStop());
            StopCoroutine(BuildingOnClickAnimationStop());
            while (destroyBar.transform.localPosition.y < -420)
            {
                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = destroyBar.transform.localPosition;
                animationSpeed += 0.1f;
                buildingAnimationPosition.y += 360 * animationSpeed * Time.deltaTime;
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
                if (destroyStopActive)
                {
                    break;
                }
                yield return null;
            }
            destroyStartActive = false;
            tutorialScript.BuildingCheckTutorial();
        }
    }
    IEnumerator BuildingOnClickAnimationStop()
    {
        if (!destroyStopActive)
        {
            destroyStopActive = true;
            StopCoroutine(BuildingOnClickAnimationStart());
            while (destroyBar.transform.localPosition.y > -640)
            {
                Vector3 buildingAnimationPosition;
                buildingAnimationPosition = destroyBar.transform.localPosition;
                animationSpeed += 0.1f;
                buildingAnimationPosition.y -= 360 * animationSpeed * Time.deltaTime;
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
            destroyStopActive = false;
        }
    }
}
