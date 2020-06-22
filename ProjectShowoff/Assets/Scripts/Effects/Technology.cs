using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technology : MonoBehaviour
{
    int lvl;

    public List<TrainStation> trainStations = new List<TrainStation>();

    public int level
    {
        get => lvl;
        set
        {
            lvl = value;

            foreach (TrainStation station in trainStations)
                station.level = lvl;
            if (lvl == 1)
                GetComponent<FlightPlanner>().StartPlanningFlights(this);
            else if(lvl > 1)
                GetComponent<SpaceOrganisation>().StartPlanningExpeditions(this);
        }
    }

    void Start()
    {
        lvl = 0;
    }

    private void OnEnable()
    {
        BuildingSystem.onBuildingPlaced += OnBuildingPlaced;
        BuildingSystem.onBuildingDestroyed += OnBuildingDestroyed;
    }

    private void OnDisable()
    {
        BuildingSystem.onBuildingPlaced -= OnBuildingPlaced;
        BuildingSystem.onBuildingDestroyed -= OnBuildingDestroyed;
    }

    void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        TrainStation trainStation = building.GetComponent<TrainStation>();
        if (trainStation != null)
        {
            trainStation.level = lvl;
            trainStations.Add(trainStation);
        }
    }

    void OnBuildingDestroyed(BuildingLocation location, BuildingPlacer buildingData, Building building)
    {
        TrainStation trainStation = building.GetComponent<TrainStation>();
        if (trainStation != null)
        {
            trainStations.Remove(trainStation);
        }
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
