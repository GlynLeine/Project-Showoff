using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public LocationType locationType;

    public GameObject buildingPrefab;

    private BuildingSystem system;

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();
    }

    public void Place()
    {
        system.PlaceBuilding(this);
    }
}
