using System.Collections;
using System.Linq;
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
    CharacterSystem characterSystem;

    public BuildingLocation GetValidTravelLocation(BuildingLocation exclude = null)
    {
        List<BuildingLocation> possibilities = new List<BuildingLocation>(closedSet);
        if (exclude != null && possibilities.Contains(exclude))
            possibilities.Remove(exclude);

        if (possibilities.Count > 0)
            return possibilities[Random.Range(0, possibilities.Count)];
        return null;
    }

    class AStarNode
    {
        public AStarNode(BuildingLocation location)
        {
            this.location = location;
        }

        public List<AStarNode> neighbours;

        public BuildingLocation location;
        public AStarNode parent = null;
        public float gScore = float.MaxValue;
        public float fScore = float.MaxValue;
    }

    private Stack<BuildingLocation> ConstructReverseAStarPath(Stack<BuildingLocation> reversePath, AStarNode current)
    {
        reversePath.Push(current.location);
        if (current.parent != null)
            return ConstructReverseAStarPath(reversePath, current.parent);
        return reversePath;
    }

    public Queue<BuildingLocation> GetPath(BuildingLocation start, BuildingLocation target)
    {
        Dictionary<BuildingLocation, AStarNode> nodes = new Dictionary<BuildingLocation, AStarNode>();
        SortedDictionary<float, List<AStarNode>> open = new SortedDictionary<float, List<AStarNode>>();

        foreach (var loc in closedSet)
            nodes.Add(loc, new AStarNode(loc));

        foreach (var loc in unoccupied)
            nodes.Add(loc, new AStarNode(loc));

        foreach (var node in nodes)
        {
            node.Value.neighbours = new List<AStarNode>();
            foreach (var neighbour in node.Value.location.neighbours)
            {
                if (nodes.ContainsKey(neighbour))
                    node.Value.neighbours.Add(nodes[neighbour]);
            }
        }

        AStarNode startNode = nodes[start];
        startNode.gScore = 0;
        startNode.fScore = (target.transform.position - start.transform.position).magnitude;
        open.Add(0, new List<AStarNode>());
        open[0].Add(startNode);

        while (open.Count != 0)
        {
            AStarNode current = open[open.Keys.First()][0];
            open[current.gScore].RemoveAt(0);
            if (open[current.gScore].Count == 0)
                open.Remove(current.gScore);

            if (current.location == target)
            {
                Stack<BuildingLocation> reversePath = new Stack<BuildingLocation>();
                reversePath = ConstructReverseAStarPath(reversePath, current);
                Queue<BuildingLocation> path = new Queue<BuildingLocation>();
                while (reversePath.Count != 0)
                {
                    path.Enqueue(reversePath.Pop());
                }

                return path;
            }

            foreach (AStarNode neighbour in current.neighbours)
            {
                float newGScore = current.gScore + (current.location.transform.position - neighbour.location.transform.position).magnitude;
                if (newGScore < neighbour.gScore)
                {
                    if (open.ContainsKey(neighbour.gScore))
                    {
                        if (open[neighbour.gScore].Contains(neighbour))
                            open[neighbour.gScore].Remove(neighbour);

                        if (open[neighbour.gScore].Count == 0)
                            open.Remove(neighbour.gScore);
                    }

                    neighbour.parent = current;
                    neighbour.gScore = newGScore;
                    neighbour.fScore = newGScore + (target.transform.position - neighbour.location.transform.position).magnitude;
                    if (!open.ContainsKey(newGScore))
                        open.Add(newGScore, new List<AStarNode>());
                    open[newGScore].Add(neighbour);
                }
            }
        }

        return null;
    }

    private void Init()
    {
        if (initialised)
            return;

        initialised = true;

        characterSystem = GetComponent<CharacterSystem>();

        foreach (var locType in locations)
            foreach (BuildingLocation location in locType.Value)
            {
                unvisited.Add(location);
            }
    }

    private void Start()
    {
        BuildingLocation[] locations = FindObjectsOfType<BuildingLocation>();
        for (int i = 0; i < locations.Length; i++)
        {
            SphereCollider collider = locations[i].gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.01f;
        }
    }

    private void Update()
    {

        Vector3 rayPos = Camera.main.transform.position;

        BuildingLocation[] locations = FindObjectsOfType<BuildingLocation>();
        foreach (var loc in locations)
        {
            if (closedSet.Contains(loc) || unoccupied.Contains(loc))
            {
                Vector3 rayDir = (loc.transform.position - rayPos).normalized;
                if (Physics.Raycast(rayPos, rayDir, out RaycastHit hit) && hit.collider == loc.GetComponent<SphereCollider>())
                    foreach (var neighbour in loc.neighbours)
                    {
                        rayDir = (neighbour.transform.position - rayPos).normalized;
                        if (Physics.Raycast(rayPos, rayDir, out hit) && hit.collider == neighbour.GetComponent<SphereCollider>())
                            if (closedSet.Contains(neighbour) || unoccupied.Contains(neighbour))
                                DrawLine(loc.transform.position, neighbour.transform.position, Color.white, 0.01f, 0.1f);
                    }
            }
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float width = 0.01f, float duration = 0.2f)
    {
        GameObject parent = GameObject.Find("/Lines");
        if (parent == null)
            parent = new GameObject("Lines");

        GameObject myLine = new GameObject("line");
        myLine.transform.parent = parent.transform;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Custom/LineShader"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    private void OnDrawGizmos()
    {
        Init();

        Vector3 rayPos = Camera.main.transform.position;

        Gizmos.color = Color.red;
        foreach (var loc in closedSet)
            if (loc != null)
            {
                Vector3 rayDir = (loc.transform.position - rayPos).normalized;
                if (Physics.Raycast(rayPos, rayDir, out RaycastHit hit) && hit.collider == loc.GetComponent<SphereCollider>())
                    Gizmos.DrawSphere(loc.transform.position, 0.02f);
            }

        Gizmos.color = Color.green;
        foreach (var loc in openSet)
            if (loc != null)
            {
                Vector3 rayDir = (loc.transform.position - rayPos).normalized;
                if (Physics.Raycast(rayPos, rayDir, out RaycastHit hit) && hit.collider == loc.GetComponent<SphereCollider>())
                    Gizmos.DrawSphere(loc.transform.position, 0.02f);
            }

        Gizmos.color = Color.blue;
        foreach (var loc in unoccupied)
            if (loc != null)
            {
                Vector3 rayDir = (loc.transform.position - rayPos).normalized;
                if (Physics.Raycast(rayPos, rayDir, out RaycastHit hit) && hit.collider == loc.GetComponent<SphereCollider>())
                    Gizmos.DrawSphere(loc.transform.position, 0.02f);
            }

        Gizmos.color = Color.white;
        foreach (var loc in unvisited)
            if (loc != null)
            {
                Vector3 rayDir = (loc.transform.position - rayPos).normalized;
                if (Physics.Raycast(rayPos, rayDir, out RaycastHit hit) && hit.collider == loc.GetComponent<SphereCollider>())
                    Gizmos.DrawSphere(loc.transform.position, 0.02f);
            }
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

        Transform parent = planet.Find("Locations");
        if (parent == null)
        {
            parent = new GameObject("Locations").transform;
            parent.parent = planet;
            parent.localPosition = Vector3.zero;
            parent.localRotation = Quaternion.identity;
        }

        location.transform.parent = parent;
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
            visited.Add(location);
        }

        foreach (BuildingLocation location in openSet)
        {
            location.parent = null;
            toCheck.Enqueue(location);
            visited.Add(location);
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

            if (!visited.Contains(location))
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
            {
                if (location.neighbours[i] != location.parent)
                    location.parent = null;

                if (location.roads[i] != null)
                    location.roads[i].SetActive(true);
            }
        }

        if (location.parent != null)
            HandlePath(location.parent);
    }

    private void ConstructBuilding(BuildingLocation location, BuildingPlacer buildingData)
    {
        GameObject building = Instantiate(buildingData.buildingPrefab, location.transform);
        building.transform.localRotation = Quaternion.identity;
        building.transform.localPosition = Vector3.zero;
        closedSet.Add(location);
        foreach (BuildingLocation neighbour in location.neighbours)
            if (unvisited.Contains(neighbour))
            {
                openSet.Add(neighbour);
                unvisited.Remove(neighbour);
            }

        GameManager.AddState(buildingData.environmentEffect, buildingData.pollutionEffect, buildingData.happinessEffect);
        characterSystem.SpawnCharacter(location);

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
