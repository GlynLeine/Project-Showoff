using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BuildingPlacer : MonoBehaviour
{
    public LocationType locationType;

    public BuildingType buildingType;
    public Button button;

    public float natureEffect;
    public float pollutionEffect;
    public float industryEffect;
    public float happinessEffect;

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
