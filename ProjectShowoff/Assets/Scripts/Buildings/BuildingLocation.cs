using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoadDictionary
{
    [SerializeField]
    private List<BuildingLocation> keys = new List<BuildingLocation>();
    [SerializeField]
    private List<Road> values = new List<Road>();

    public int Count => keys.Count;

    public Road this[BuildingLocation key]
    {
        get
        {
            if (keys.Contains(key))
                return values[keys.IndexOf(key)];
            throw new KeyNotFoundException();
        }
        set
        {
            if (ContainsKey(key))
                values[keys.IndexOf(key)] = value;
            else
                throw new KeyNotFoundException();
        }
    }

    public Road this[int index]
    {
        get
        {
            if (index >= 0 && index < values.Count)
                return values[index];
            throw new System.ArgumentOutOfRangeException();
        }

        set
        {
            if (index >= 0 && index < values.Count)
                values[index] = value;
            else
                throw new System.ArgumentOutOfRangeException();
        }
    }

    public bool ContainsKey(BuildingLocation key)
    {
        return keys.Contains(key);
    }

    public void Add(BuildingLocation key, Road value)
    {
        keys.Add(key);
        values.Add(value);
    }

    public void Remove(BuildingLocation key)
    {
        int index = keys.IndexOf(key);
        if(index < 0 || index >= keys.Count)
            return;
        keys.RemoveAt(index);
        values.RemoveAt(index);
    }
}

public class BuildingLocation : MonoBehaviour
{
    BuildingSystem system;

    [HideInInspector]
    public BuildingLocation parent;

    public LocationType locationType;

    public List<BuildingLocation> neighbours = new List<BuildingLocation>();
    [SerializeField]
    public RoadDictionary roads = new RoadDictionary();

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();
        if (system != null)
        {
            system.ReportLocation(this);
        }

        transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.Self);
    }
}
