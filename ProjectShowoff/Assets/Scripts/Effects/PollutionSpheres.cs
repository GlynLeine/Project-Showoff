using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionSpheres : MonoBehaviour
{
    public GameObject pollutionSpherePrefab;
    public Material pollutionMaterial;

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
        if (buildingData.buildingType == BuildingType.Factory || buildingData.buildingType == BuildingType.CoalMine)
            Instantiate(pollutionSpherePrefab, location.transform);
    }

    private void Update()
    {
        pollutionMaterial.SetFloat("_Growth", GameManager.smoothstep(400f, 2400f, GameManager.pollution));
    }
}
