using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLocation : MonoBehaviour
{
    BuildingSystem system;

    [HideInInspector]
    public BuildingLocation parent;

    public LocationType locationType;

    public List<BuildingLocation> neighbours = new List<BuildingLocation>();
    public List<GameObject> roads = new List<GameObject>();

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();
        if (system != null)
        {
            system.ReportLocation(this);
        }
    }
}
