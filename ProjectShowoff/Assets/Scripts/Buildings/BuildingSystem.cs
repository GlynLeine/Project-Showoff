using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType
{
    Rural, Coastal
}

public enum BuildingType
{
    Factory, TrainStation, CoalMine, OilRig, SolarFarm, Harbor, NatureReserve
}

public class BuildingSystem : MonoBehaviour
{
    Dictionary<LocationType, List<BuildingLocation>> locations = new Dictionary<LocationType, List<BuildingLocation>>();

    List<BuildingLocation> destroyed = new List<BuildingLocation>();

    List<BuildingLocation> unvisited = new List<BuildingLocation>();
    List<BuildingLocation> closedSet = new List<BuildingLocation>();
    List<BuildingLocation> openSet = new List<BuildingLocation>();
    List<BuildingLocation> unoccupied = new List<BuildingLocation>();

    public Transform planet;
    public GameObject buildingConstructorPrefab;

    bool initialised = false;
    CharacterSystem characterSystem;

    public delegate void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building);
    public static OnBuildingPlaced onBuildingPlaced;

    public UnityEngine.UI.Toggle DestructionToggle;
    bool destroy = false;

    public ClickableBarPopup buildUI;
    public BuildingLocation startLocation;
    BuildingLocation selectedLocation;

    //bool place = false;

    public void ToggleDestroyMode()
    {
        destroy = DestructionToggle.isOn;
    }

    private void Awake()
    {
        foreach (Building building in FindObjectsOfType<Building>())
            building.gameObject.SetActive(false);

        locations = new Dictionary<LocationType, List<BuildingLocation>>();
    }

    private void Update()
    {
        if (InputRedirect.tapped)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputRedirect.inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!destroy)
                {
                    BuildingConstructor constructor = hit.collider.GetComponent<BuildingConstructor>();
                    if (constructor != null)
                    {
                        selectedLocation = constructor.location;

                        buildUI.GetType().GetMethod(selectedLocation.locationType.ToString() + "Start").Invoke(buildUI, new object[] { });

                        //HandlePath(constructor);
                        //ConstructBuilding(constructor.location, constructor.buildingData);
                        //foreach (BuildingConstructor buildingConstructor in FindObjectsOfType<BuildingConstructor>())
                        //    Destroy(buildingConstructor.gameObject);
                    }
                }
                else if (hit.collider.transform.parent != null && hit.collider.transform.parent.parent != null)
                {
                    BuildingLocation location = hit.collider.transform.parent.parent.gameObject.GetComponent<BuildingLocation>();
                    if (location != null)
                    {
                        DestroyBuilding(location);
                        DestructionToggle.isOn = false;
                        destroy = false;
                    }
                }
            }
        }

        //if (place && InputRedirect.tapped)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(InputRedirect.inputPos);
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        BuildingConstructor constructor = hit.collider.GetComponent<BuildingConstructor>();
        //        if (constructor != null)
        //        {
        //            HandlePath(constructor);
        //            ConstructBuilding(constructor.location, constructor.buildingData);
        //            foreach (BuildingConstructor buildingConstructor in FindObjectsOfType<BuildingConstructor>())
        //                Destroy(buildingConstructor.gameObject);
        //        }
        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        Init();

        int count = 0;
        Gizmos.color = Color.black;
        foreach (BuildingLocation location in destroyed)
        {
            count = destroyed.FindAll(delegate (BuildingLocation loc)
            {
                return loc == location;
            }).Count;
            if (count > 1)
                Debug.Log("destroyed " + count);
            Gizmos.DrawSphere(location.transform.position, 0.125f);
        }

        Gizmos.color = Color.red;
        foreach (BuildingLocation location in closedSet)
        {
            count = closedSet.FindAll(delegate (BuildingLocation loc)
            {
                return loc == location;
            }).Count;
            if (count > 1)
                Debug.Log("closedSet " + count);
            Gizmos.DrawSphere(location.transform.position, 0.1f);
        }

        Gizmos.color = Color.green;
        foreach (BuildingLocation location in openSet)
        {
            count = openSet.FindAll(delegate (BuildingLocation loc)
            {
                return loc == location;
            }).Count;
            if (count > 1)
                Debug.Log("openSet " + count);
            Gizmos.DrawSphere(location.transform.position, 0.075f);
        }

        Gizmos.color = Color.blue;
        foreach (BuildingLocation location in unoccupied)
        {
            count = unoccupied.FindAll(delegate (BuildingLocation loc)
            {
                return loc == location;
            }).Count;
            if (count > 1)
                Debug.Log("unoccupied " + count);
            Gizmos.DrawSphere(location.transform.position, 0.05f);
        }

        Gizmos.color = Color.white;
        foreach (BuildingLocation location in unvisited)
        {
            count = unvisited.FindAll(delegate (BuildingLocation loc)
            {
                return loc == location;
            }).Count;
            if (count > 1)
                Debug.Log("unvisited " + count);
            Gizmos.DrawSphere(location.transform.position, 0.025f);
        }
    }

    #region destruction and recovery
    public void RecoverLocation(BuildingLocation location)
    {
        if (destroyed.Contains(location))
        {
            destroyed.Remove(location);

            int closedNeighbours = 0;
            foreach (BuildingLocation neighbour in location.neighbours)
                if (closedSet.Contains(neighbour))
                    closedNeighbours++;

            if (closedNeighbours > 0)
            {
                if (closedNeighbours >= 2)
                    unoccupied.Add(location);
                else
                    openSet.Add(location);
            }
            else
                unvisited.Add(location);
        }
    }

    public void DestroyLocation(BuildingLocation location)
    {
        if (destroyed.Contains(location))
            return;

        if (DestroyBuilding(location))
            GameManager.buildingsFlooded++;

        if (openSet.Contains(location))
            openSet.Remove(location);

        if (unoccupied.Contains(location))
            unoccupied.Remove(location);

        if (unvisited.Contains(location))
            unvisited.Remove(location);

        destroyed.Add(location);
    }

    public bool DestroyBuilding(BuildingLocation location)
    {
        if (!closedSet.Contains(location))
            return false;

        GameManager.buildingsDestroyed++;

        Building building = location.GetComponentInChildren<Building>();
        if (building == null)
            return false;

        closedSet.Remove(location);

        int closedNeighbours = 0;
        foreach (BuildingLocation neighbour in location.neighbours)
        {
            if (closedSet.Contains(neighbour))
                closedNeighbours++;
            else if (!unvisited.Contains(neighbour)) // check destroyed
            {
                int closedfurterNeighbours = 0;

                foreach (BuildingLocation furterNeighbour in neighbour.neighbours)
                    if (furterNeighbour != location && closedSet.Contains(furterNeighbour))
                    {
                        closedfurterNeighbours++;
                    }

                if (closedfurterNeighbours <= 1 && unoccupied.Contains(neighbour))
                {
                    unoccupied.Remove(neighbour);
                    unvisited.Add(neighbour);
                }

                if (openSet.Contains(neighbour) && closedfurterNeighbours <= 0)
                {
                    openSet.Remove(neighbour);
                    unvisited.Add(neighbour);
                }
            }
        }

        if (closedNeighbours > 0)
        {
            if (closedNeighbours >= 2)
            {
                if (!unoccupied.Contains(location))
                    unoccupied.Add(location);
            }
            else if (!openSet.Contains(location))
                openSet.Add(location);
        }
        else if (!unvisited.Contains(location))
            unvisited.Add(location);

        for (int i = 0; i < location.roads.Count; i++)
            location.roads[i].gameObject.SetActive(false);

        GameObject source = location.GetType().GetField(building.buildingType.ToString()).GetValue(location) as GameObject;

        if (source.scene.name == null || source.scene.rootCount == 0)
            Destroy(building.gameObject);
        else
            building.gameObject.SetActive(false);

        characterSystem.DespawnCharacter(location);

        GameManager.AddState(-building.natureRemovalEffect, -building.pollutionRemovalEffect, -building.industryRemovalEffect);

        return true;
    }
    #endregion

    #region character path finding
    public bool IsValidTravelLocation(BuildingLocation location)
    {
        return closedSet.Contains(location);
    }

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
        {
            nodes.Add(loc, new AStarNode(loc));
        }

        foreach (var node in nodes)
        {
            node.Value.neighbours = new List<AStarNode>();
            foreach (var neighbour in node.Value.location.neighbours)
            {
                if (nodes.ContainsKey(neighbour))
                    node.Value.neighbours.Add(nodes[neighbour]);
            }
        }

        if (!nodes.ContainsKey(start))
        {
            Debug.Log("dafuq?? " + start);
            return null;
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
    #endregion

    private void Init()
    {
        if (initialised)
            return;

        initialised = true;

        characterSystem = GetComponent<CharacterSystem>();
        unvisited = new List<BuildingLocation>();

        foreach (var locType in locations)
            foreach (BuildingLocation location in locType.Value)
            {
                unvisited.Add(location);
            }
    }

    public void ReportLocation(BuildingLocation location)
    {
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

        Transform ocean = planet.Find("Ocean");
        SphereCollider oceanCollider = ocean.GetComponent<SphereCollider>();
        location.oceanCenter = ocean.TransformPoint(oceanCollider.center);
        location.oceanCollider = oceanCollider;
        location.ocean = ocean;

        location.transform.parent = parent;
        location.transform.up = (location.transform.position - planet.position).normalized;
        locations[location.locationType].Add(location);

        if (location == startLocation)
            EnableLocation(location, true);
    }

    private void EnableLocation(BuildingLocation location, bool enable)
    {
        BuildingConstructor constructor = location.GetComponentInChildren<BuildingConstructor>(true);
        if (constructor == null)
        {
            constructor = Instantiate(buildingConstructorPrefab, location.transform).GetComponent<BuildingConstructor>();
            constructor.location = location;
        }

        constructor.gameObject.SetActive(enable);
    }

    public void PlaceBuilding(BuildingPlacer buildingData)
    {
        Init();
        EnableLocation(selectedLocation, false);
        ConstructBuilding(selectedLocation, buildingData);
        foreach (BuildingLocation location in GetPossibleBuildingLocations())
        {
            EnableLocation(location, true);
        }
    }

    public List<BuildingLocation> GetPossibleBuildingLocations()
    {
        if (unoccupied.Count == 0 && openSet.Count == 0)
        {
            if (unvisited.Count == 0)
            {
                Debug.Log("No space");
                return null;
            }

            if (closedSet.Count > 0 || GameManager.buildingsPlaced == 0)
                GameManager.continentsDiscovered++;

            return unvisited;
        }

        List<BuildingLocation> options = new List<BuildingLocation>();
        Queue<BuildingLocation> toCheck = new Queue<BuildingLocation>();
        List<BuildingLocation> visited = new List<BuildingLocation>();

        foreach (BuildingLocation location in unoccupied)
        {
            location.parent = null;
            toCheck.Enqueue(location);
            visited.Add(location);

            location.path = null;
            options.Add(location);
        }

        foreach (BuildingLocation location in openSet)
        {
            location.parent = null;
            toCheck.Enqueue(location);
            visited.Add(location);

            location.path = null;
            options.Add(location);
        }

        return options;
    }

    private void ConstructBuilding(BuildingLocation location, BuildingPlacer buildingData)
    {
        if (unoccupied.Contains(location))
            unoccupied.Remove(location);
        if (openSet.Contains(location))
            openSet.Remove(location);
        if (unvisited.Contains(location))
            unvisited.Remove(location);

        GameObject source = location.GetType().GetField(buildingData.buildingType.ToString()).GetValue(location) as GameObject;
        GameObject buildingObj;
        if (source.scene.name == null || source.scene.rootCount == 0)
        {
            buildingObj = Instantiate(source, location.transform);
            buildingObj.transform.localRotation = Quaternion.identity;
            buildingObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            buildingObj = source;
            buildingObj.SetActive(true);
            buildingObj.transform.parent = location.transform;
        }

        Building building = buildingObj.GetComponent<Building>();
        building.buildingType = buildingData.buildingType;

        building.natureRemovalEffect = buildingData.natureEffect;
        building.pollutionRemovalEffect = buildingData.pollutionEffect;
        building.industryRemovalEffect = buildingData.industryEffect;

        if (!closedSet.Contains(location))
            closedSet.Add(location);
        foreach (BuildingLocation neighbour in location.neighbours)
            if (unvisited.Contains(neighbour))
            {
                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
                unvisited.Remove(neighbour);
            }

        GameManager.AddState(buildingData.natureEffect, buildingData.pollutionEffect, buildingData.industryEffect);
        characterSystem.SpawnCharacter(location);

        if (buildingData.natureEffect > 0)
            GameManager.natureBuildings++;

        if (location.locationType == LocationType.Rural)
            GameManager.ruralBuildings++;
        else
            GameManager.coastalBuildings++;

        GameManager.buildingsPlaced++;

        int buildingCount = GameManager.buildingsPlaced - GameManager.buildingsDestroyed;
        if (buildingCount > GameManager.maxBuildings)
            GameManager.maxBuildings = buildingCount;

        onBuildingPlaced?.Invoke(location, buildingData, building);
    }

    #region Faulty placement system
    //public bool StartBuildingProcess(BuildingPlacer buildingData)
    //{
    //    List<BuildingLocation> options = GetPossibleBuildingLocations(buildingData);
    //    if (options != null && options.Count > 0)
    //    {
    //        foreach (BuildingLocation location in options)
    //        {
    //            GameObject constuctorObject = Instantiate(buildingConstructorPrefab, location.transform);
    //            constuctorObject.transform.localPosition = Vector3.zero;

    //            BuildingConstructor constructor = constuctorObject.GetComponent<BuildingConstructor>();
    //            constructor.location = location;
    //            constructor.buildingData = buildingData;
    //        }
    //        place = true;
    //    }

    //    return place;
    //}

    //public List<BuildingLocation> GetPossibleBuildingLocations(BuildingPlacer buildingData)
    //{
    //    Init();

    //    if (unoccupied.Count == 0 && openSet.Count == 0)
    //    {
    //        if (unvisited.Count == 0)
    //        {
    //            Debug.Log("No space");
    //            return null;
    //        }

    //        if (closedSet.Count > 0 || GameManager.buildingsPlaced == 0)
    //            GameManager.continentsDiscovered++;

    //        List<BuildingLocation> locationsOfType = locations[buildingData.locationType];
    //        for (int i = 0; i < locationsOfType.Count; i++)
    //        {
    //            locationsOfType[i].path = null;
    //            if (closedSet.Contains(locationsOfType[i]))
    //            {
    //                locationsOfType.RemoveAt(i);
    //                i--;
    //            }
    //        }

    //        return locationsOfType;
    //    }

    //    List<BuildingLocation> options = new List<BuildingLocation>();

    //    Queue<BuildingLocation> toCheck = new Queue<BuildingLocation>();
    //    List<BuildingLocation> visited = new List<BuildingLocation>();

    //    foreach (BuildingLocation location in unoccupied)
    //    {
    //        location.parent = null;
    //        toCheck.Enqueue(location);
    //        visited.Add(location);

    //        if (location.locationType == buildingData.locationType)
    //        {
    //            location.path = null;
    //            options.Add(location);
    //        }
    //    }

    //    foreach (BuildingLocation location in openSet)
    //    {
    //        location.parent = null;
    //        toCheck.Enqueue(location);
    //        visited.Add(location);

    //        if (location.locationType == buildingData.locationType)
    //        {
    //            location.path = null;
    //            options.Add(location);
    //        }
    //    }

    //    if (options.Count > 0)
    //        return options;

    //    while (toCheck.Count > 0)
    //    {
    //        BuildingLocation location = toCheck.Dequeue();
    //        if (location.locationType == buildingData.locationType)
    //        {
    //            location.path = new Queue<BuildingLocation>();
    //            ConstructPath(location, ref location.path);
    //            options.Add(location);
    //        }

    //        if (!visited.Contains(location))
    //            visited.Add(location);

    //        foreach (BuildingLocation neighbour in location.neighbours)
    //        {
    //            if (!visited.Contains(neighbour) && !closedSet.Contains(neighbour))
    //            {
    //                visited.Add(neighbour);
    //                neighbour.parent = location;
    //                toCheck.Enqueue(neighbour);
    //            }
    //        }
    //    }

    //    if (options.Count <= 0)
    //    {
    //        options = locations[buildingData.locationType];
    //        for (int i = 0; i < options.Count; i++)
    //        {
    //            options[i].path = null;
    //            if (closedSet.Contains(options[i]))
    //            {
    //                options.RemoveAt(i);
    //                i--;
    //            }
    //        }
    //    }

    //    return options;
    //}

    //private void ConstructPath(BuildingLocation location, ref Queue<BuildingLocation> path)
    //{
    //    if (location.parent != null)
    //        ConstructPath(location.parent, ref path);

    //    path.Enqueue(location);
    //}

    //private void HandlePath(BuildingConstructor constructor)
    //{
    //    BuildingLocation end = constructor.location;
    //    if (end.path == null)
    //        return;

    //    while (end.path.Count > 0)
    //    {
    //        BuildingLocation location = end.path.Dequeue();

    //        if (unoccupied.Contains(location))
    //            unoccupied.Remove(location);
    //        else if (openSet.Contains(location))
    //            openSet.Remove(location);
    //        else if (unvisited.Contains(location))
    //            unvisited.Remove(location);

    //        unoccupied.Add(location);

    //        for (int i = 0; i < location.neighbours.Count; i++)
    //        {
    //            if (location.roads.Count <= i)
    //                break;
    //            if (unoccupied.Contains(location.neighbours[i]) || closedSet.Contains(location.neighbours[i]))
    //            {
    //                if (location.neighbours[i] != location.parent)
    //                    location.parent = null;

    //                if (location.roads[location.neighbours[i]] != null)
    //                    location.roads[location.neighbours[i]].gameObject.SetActive(true);
    //            }
    //        }
    //    }
    //}
    #endregion

    #region Old placement system
    //public bool PlaceBuilding(BuildingPlacer buildingData)
    //{
    //    Init();

    //    if (unoccupied.Count == 0 && openSet.Count == 0)
    //    {
    //        if (unvisited.Count == 0)
    //        {
    //            Debug.Log("No space");
    //            return false;
    //        }

    //        if (GameManager.buildingsPlaced == 0)
    //            GameManager.continentsDiscovered++;

    //        if (closedSet.Count > 0)
    //            GameManager.continentsDiscovered++;

    //        return PlaceRandomBuilding(buildingData);
    //    }

    //    Queue<BuildingLocation> toCheck = new Queue<BuildingLocation>();
    //    List<BuildingLocation> visited = new List<BuildingLocation>();

    //    foreach (BuildingLocation location in unoccupied)
    //    {
    //        location.parent = null;
    //        toCheck.Enqueue(location);
    //        visited.Add(location);
    //    }

    //    foreach (BuildingLocation location in openSet)
    //    {
    //        location.parent = null;
    //        toCheck.Enqueue(location);
    //        visited.Add(location);
    //    }

    //    BuildingLocation end = null;
    //    while (toCheck.Count > 0)
    //    {
    //        BuildingLocation location = toCheck.Dequeue();
    //        if (location.locationType == buildingData.locationType)
    //        {
    //            end = location;
    //            break;
    //        }

    //        if (!visited.Contains(location))
    //            visited.Add(location);

    //        foreach (BuildingLocation neighbour in location.neighbours)
    //        {
    //            if (!visited.Contains(neighbour) && !closedSet.Contains(neighbour))
    //            {
    //                visited.Add(neighbour);
    //                neighbour.parent = location;
    //                toCheck.Enqueue(neighbour);
    //            }
    //        }
    //    }

    //    if (end != null)
    //    {
    //        for (int i = 0; i < end.neighbours.Count; i++)
    //        {
    //            if (end.roads.Count <= i)
    //                break;
    //            if (unoccupied.Contains(end.neighbours[i]) || closedSet.Contains(end.neighbours[i]) || end.neighbours[i] == end.parent)
    //                if (end.roads[end.neighbours[i]] != null)
    //                    end.roads[end.neighbours[i]].gameObject.SetActive(true);
    //        }

    //        if (end.parent != null)
    //            HandlePath(end.parent);
    //        ConstructBuilding(end, buildingData);
    //        return true;
    //    }
    //    else
    //    {
    //        if (PlaceRandomBuilding(buildingData))
    //        {
    //            GameManager.continentsDiscovered++;
    //            return true;
    //        }

    //        Debug.Log("No more of this type left");
    //        return false;
    //    }
    //}

    //private void HandlePath(BuildingLocation location)
    //{
    //    if (unoccupied.Contains(location))
    //        unoccupied.Remove(location);
    //    else if (openSet.Contains(location))
    //        openSet.Remove(location);
    //    else if (unvisited.Contains(location))
    //        unvisited.Remove(location);

    //    unoccupied.Add(location);

    //    for (int i = 0; i < location.neighbours.Count; i++)
    //    {
    //        if (location.roads.Count <= i)
    //            break;
    //        if (unoccupied.Contains(location.neighbours[i]) || closedSet.Contains(location.neighbours[i]))
    //        {
    //            if (location.neighbours[i] != location.parent)
    //                location.parent = null;

    //            if (location.roads[location.neighbours[i]] != null)
    //                location.roads[location.neighbours[i]].gameObject.SetActive(true);
    //        }
    //    }

    //    if (location.parent != null)
    //        HandlePath(location.parent);
    //}

    //private void ConstructBuilding(BuildingLocation location, BuildingPlacer buildingData)
    //{
    //    if (unoccupied.Contains(location))
    //        unoccupied.Remove(location);
    //    if (openSet.Contains(location))
    //        openSet.Remove(location);
    //    if (unvisited.Contains(location))
    //        unvisited.Remove(location);

    //    GameObject source = location.GetType().GetField(buildingData.buildingType.ToString()).GetValue(location) as GameObject;
    //    GameObject buildingObj;
    //    if (source.scene.name == null || source.scene.rootCount == 0)
    //    {
    //        buildingObj = Instantiate(source, location.transform);
    //        buildingObj.transform.localRotation = Quaternion.identity;
    //        buildingObj.transform.localPosition = Vector3.zero;
    //    }
    //    else
    //    {
    //        buildingObj = source;
    //        buildingObj.SetActive(true);
    //        buildingObj.transform.parent = location.transform;
    //    }

    //    Building building = buildingObj.GetComponent<Building>();
    //    building.buildingType = buildingData.buildingType;

    //    building.natureRemovalEffect = buildingData.natureEffect;
    //    building.pollutionRemovalEffect = buildingData.pollutionEffect;
    //    building.industryRemovalEffect = buildingData.industryEffect;

    //    if (!closedSet.Contains(location))
    //        closedSet.Add(location);
    //    foreach (BuildingLocation neighbour in location.neighbours)
    //        if (unvisited.Contains(neighbour))
    //        {
    //            if (!openSet.Contains(neighbour))
    //                openSet.Add(neighbour);
    //            unvisited.Remove(neighbour);
    //        }

    //    GameManager.AddState(buildingData.natureEffect, buildingData.pollutionEffect, buildingData.industryEffect);
    //    characterSystem.SpawnCharacter(location);

    //    if (buildingData.natureEffect > 0)
    //        GameManager.natureBuildings++;

    //    if (location.locationType == LocationType.Rural)
    //        GameManager.ruralBuildings++;
    //    else
    //        GameManager.coastalBuildings++;

    //    GameManager.buildingsPlaced++;

    //    int buildingCount = GameManager.buildingsPlaced - GameManager.buildingsDestroyed;
    //    if (buildingCount > GameManager.maxBuildings)
    //        GameManager.maxBuildings = buildingCount;

    //    onBuildingPlaced?.Invoke(location, buildingData, building);
    //}

    //private bool PlaceRandomBuilding(BuildingPlacer buildingData)
    //{
    //    List<BuildingLocation> locationsOfType = locations[buildingData.locationType];
    //    for (int i = 0; i < locationsOfType.Count; i++)
    //        if (closedSet.Contains(locationsOfType[i]))
    //        {
    //            locationsOfType.RemoveAt(i);
    //            i--;
    //        }

    //    if (locationsOfType.Count > 0)
    //    {
    //        BuildingLocation location = locationsOfType[Random.Range(0, locationsOfType.Count)];

    //        Debug.Log("Got random location");

    //        ConstructBuilding(location, buildingData);
    //        return true;
    //    }

    //    Debug.Log("no space");
    //    return false;
    //}
    #endregion
}
