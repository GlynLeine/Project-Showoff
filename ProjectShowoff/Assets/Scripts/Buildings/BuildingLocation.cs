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

    public BuildingLocation KeyAt(int index)
    {
        if (index < 0 || index >= keys.Count)
            return null;

        return keys[index];
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
        if (index < 0 || index >= keys.Count)
            return;
        keys.RemoveAt(index);
        values.RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= keys.Count)
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
    [HideInInspector]
    public Queue<BuildingLocation> path;
    [HideInInspector]
    public Vector3 oceanCenter;
    [HideInInspector]
    public Transform ocean;
    [HideInInspector]
    public SphereCollider oceanCollider;

    public LocationType locationType;
    public GameObject Factory;
    public GameObject TrainStation;
    public GameObject CoalMine;
    public GameObject OilRig;
    public GameObject SolarFarm;
    public GameObject Harbor;
    public GameObject NatureReserve;

    public List<BuildingLocation> neighbours = new List<BuildingLocation>();
    [SerializeField]
    public RoadDictionary roads = new RoadDictionary();

    public LocationState state;

    public void Revalidate()
    {
        foreach (BuildingLocation neighbour in neighbours)
        {
            if (!neighbour.neighbours.Contains(this))
            {
                neighbour.neighbours.Add(this);
                neighbour.roads.Add(this, roads[neighbour]);
            }
        }

        for (int i = 0; i < roads.Count; i++)
        {
            if (roads[i] == null)
            {
                roads.RemoveAt(i);
                i--;
            }
        }
    }

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();
        if (system != null)
        {
            system.ReportLocation(this);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, oceanCenter) < oceanCollider.radius * ocean.localScale.x)
        {
            system.DestroyLocation(this);
        }
        else
        {
            system.RecoverLocation(this);
        }
    }
}
