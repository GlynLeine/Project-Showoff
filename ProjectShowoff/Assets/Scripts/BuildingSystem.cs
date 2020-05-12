﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType
{
    Rural, Coastal
}

public class BuildingSystem : MonoBehaviour
{
    Dictionary<LocationType, List<BuildingLocation>> locations = new Dictionary<LocationType, List<BuildingLocation>>();

    List<BuildingLocation> unvisited = new List<BuildingLocation>();
    List<BuildingLocation> closedSet = new List<BuildingLocation>();
    List<BuildingLocation> openSet = new List<BuildingLocation>();
    List<BuildingLocation> unoccupied = new List<BuildingLocation>();

    public Transform planet;

    bool initialised = false;

    private void Init()
    {
        if (initialised)
            return;

        initialised = true;

        foreach (var locType in locations)
            foreach (BuildingLocation location in locType.Value)
            {
                unvisited.Add(location);
            }
    }


    private void OnDrawGizmos()
    {
        Init();

        Gizmos.color = Color.red;
        foreach (var loc in closedSet)
            if (loc != null)
                Gizmos.DrawSphere(loc.transform.position, 0.1f);

        Gizmos.color = Color.green;
        foreach (var loc in openSet)
            if (loc != null)
                Gizmos.DrawSphere(loc.transform.position, 0.1f);

        Gizmos.color = Color.blue;
        foreach (var loc in unoccupied)
            if (loc != null)
                Gizmos.DrawSphere(loc.transform.position, 0.1f);

        Gizmos.color = Color.white;
        foreach (var loc in unvisited)
            if (loc != null)
                Gizmos.DrawSphere(loc.transform.position, 0.1f);
    }


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

    public bool PlaceBuilding(BuildingPlacer buildingData)
    {
        Init();

        if (unvisited.Count == 0 && openSet.Count == 0 && unoccupied.Count == 0)
        {
            Debug.Log("No space");
            return false;
        }

        if (closedSet.Count == 0)
        {
            PlaceRandomBuilding(buildingData);
            return true;
        }

        Queue<BuildingLocation> toCheck = new Queue<BuildingLocation>();
        List<BuildingLocation> visited = new List<BuildingLocation>();

        foreach (BuildingLocation location in unoccupied)
        {
            location.parent = null;
            toCheck.Enqueue(location);
        }

        foreach (BuildingLocation location in openSet)
        {
            location.parent = null;
            toCheck.Enqueue(location);
        }

        BuildingLocation end = null;
        while (toCheck.Count > 0)
        {
            BuildingLocation location = toCheck.Dequeue();
            if (location.locationType == buildingData.locationType)
            {
                end = location;
                break;
            }
            visited.Add(location);

            foreach (BuildingLocation neighbour in location.neighbours)
            {
                if (!visited.Contains(neighbour) && !closedSet.Contains(neighbour))
                {
                    visited.Add(neighbour);
                    neighbour.parent = location;
                    toCheck.Enqueue(neighbour);
                }
            }
        }

        if (end != null)
        {
            if (unoccupied.Contains(end))
                unoccupied.Remove(end);
            else if (openSet.Contains(end))
                openSet.Remove(end);
            else if (unvisited.Contains(end))
                unvisited.Remove(end);

            for (int i = 0; i < end.neighbours.Count; i++)
            {
                if (end.roads.Count <= i)
                    break;
                if (unoccupied.Contains(end.neighbours[i]) || closedSet.Contains(end.neighbours[i]) || end.neighbours[i] == end.parent)
                    if (end.roads[i] != null)
                        end.roads[i].SetActive(true);
            }

            if (end.parent != null)
                HandlePath(end.parent);
            ConstructBuilding(end, buildingData);
            return true;
        }
        else
        {
            Debug.Log("No more of this type left");
            return false;
        }
    }

    private void HandlePath(BuildingLocation location)
    {
        if (unoccupied.Contains(location))
            unoccupied.Remove(location);
        else if (openSet.Contains(location))
            openSet.Remove(location);
        else if (unvisited.Contains(location))
            unvisited.Remove(location);

        unoccupied.Add(location);

        for (int i = 0; i < location.neighbours.Count; i++)
        {
            if (location.roads.Count <= i)
                break;
            if (unoccupied.Contains(location.neighbours[i]) || closedSet.Contains(location.neighbours[i]))
                if (location.roads[i] != null)
                    location.roads[i].SetActive(true);
        }

        if (location.parent != null)
            HandlePath(location.parent);
    }

    private void ConstructBuilding(BuildingLocation location, BuildingPlacer buildingData)
    {
        Instantiate(buildingData.buildingPrefab, location.transform);
        closedSet.Add(location);
        foreach (BuildingLocation neighbour in location.neighbours)
            if (unvisited.Contains(neighbour))
            {
                openSet.Add(neighbour);
                unvisited.Remove(neighbour);
            }

        GameManager.AddState(buildingData.environmentEffect, buildingData.pollutionEffect, buildingData.happinessEffect);

        Debug.Log("placed a building on a " + buildingData.locationType.ToString() + " location.");
    }

    private void PlaceRandomBuilding(BuildingPlacer buildingData)
    {
        List<BuildingLocation> locationsOfType = locations[buildingData.locationType];
        if (locationsOfType.Count > 0)
        {
            BuildingLocation location = locationsOfType[Random.Range(0, locationsOfType.Count)];
            unvisited.Remove(location);
            ConstructBuilding(location, buildingData);
        }
        else
        {
            Debug.Log("no space");
        }
    }

}
