using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionMusic : MonoBehaviour
{
    [FMODUnity.EventRef] 
    private FMOD.Studio.EventInstance mainMusic;
    private string pollutionParameter = "Pollution";
    private string lifeBegins = "Life Begins";
    private int buildingCounter;

    void Start()
    {
        mainMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Main Music");
        mainMusic.start();
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    // called in every flame closest to one second later, as pollution is a slow variable to update so no need to match the music one to one
    IEnumerator SlowUpdate()
    {
        while (true)
        {
            Debug.Log("called too");
            mainMusic.setParameterByName(pollutionParameter,Mathf.Clamp(GameManager.pollution/20,0,100));
            yield return new WaitForSeconds(1);
            if (!gameObject.activeSelf)
            {
                break;
            }
        }
    }

    private void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        buildingCounter += 1;
        if (buildingCounter >= 3)
        {
            mainMusic.setParameterByName(lifeBegins, 1);
            Debug.Log("called");
            StartCoroutine(SlowUpdate());
            BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
        }
    }
}
