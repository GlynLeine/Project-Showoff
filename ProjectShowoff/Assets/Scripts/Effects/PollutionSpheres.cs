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
        {
            foreach (Transform child in building.transform)
                if (child.tag == "PollutionSphere")
                    return;

            Instantiate(pollutionSpherePrefab, building.transform);
        }
    }

    private void Update()
    {
        pollutionMaterial.SetFloat("_Growth", GameManager.lerp(0.35f, 1f, GameManager.smoothstep(400f, 2400f, GameManager.pollution)));
    }
}
