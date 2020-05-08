using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType
{
    Rural, Coastal
}

public class BuildingSystem : MonoBehaviour
{
    Dictionary<LocationType, List<BuildingLocation>> locations = new Dictionary<LocationType, List<BuildingLocation>>();

    public Transform planet;

    public void ReportLocation(BuildingLocation location)
    {
        foreach (var locType in locations)
        {
            if (locType.Value.Contains(location))
                locType.Value.Remove(location);
        }

        if (!locations.ContainsKey(location.locationType))
            locations.Add(location.locationType, new List<BuildingLocation>());

        location.transform.parent = planet;
        location.transform.up = (location.transform.position - planet.position).normalized;
        locations[location.locationType].Add(location);
    }

    public void PlaceBuilding(BuildingPlacer buildingData)
    {
        List<BuildingLocation> locationsOfType = locations[buildingData.locationType];
        if (locationsOfType.Count > 0)
        {
            BuildingLocation location = locationsOfType[Random.Range(0, locationsOfType.Count)];
            Instantiate(buildingData.buildingPrefab, location.transform);
            locationsOfType.Remove(location);

            GameManager.AddState(buildingData.environmentEffect, buildingData.pollutionEffect, buildingData.happinessEffect);

            Debug.Log("placed a building on a " + buildingData.locationType.ToString() + " location.");
        }
        else
        {
            Debug.Log("no space");
        }
    }
}
