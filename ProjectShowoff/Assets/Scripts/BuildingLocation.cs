using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLocation : MonoBehaviour
{
    BuildingSystem system;

    public LocationType locationType;

    public List<BuildingLocation> neighbours;

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();
        if (system != null)
        {
            system.ReportLocation(this);
        }
    }
}
