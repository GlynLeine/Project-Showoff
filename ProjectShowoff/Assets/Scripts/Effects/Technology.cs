using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technology : MonoBehaviour
{
    int lvl;

    int level
    {
        get => lvl;
        set
        {
            lvl = value;

            foreach (TrainStation station in FindObjectsOfType<TrainStation>())
                station.level = lvl;
        }
    }

    void Start()
    {
        lvl = 0;
    }

    private void OnEnable()
    {
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
    }

    void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        TrainStation trainStation = building.GetComponent<TrainStation>();
        if (trainStation != null)
            trainStation.level = lvl;
    }

    private void Update()
    {
        if (GameManager.industry < 15)
        {
            if (lvl != 0)
                level = 0;
        }
        else if (GameManager.industry < 30)
        {
            if (lvl != 1)
                level = 1;
        }
        else
        {
            if (lvl != 2)
                level = 2;
        }
    }
}
