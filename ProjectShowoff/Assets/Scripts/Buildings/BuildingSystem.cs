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

public enum LocationState
{
    Open, Closed, Unvisited, Destroyed
}

public class BuildingSystem : MonoBehaviour
{
    Dictionary<LocationType, List<BuildingLocation>> locations = new Dictionary<LocationType, List<BuildingLocation>>();

    List<BuildingLocation> destroyed = new List<BuildingLocation>();

    List<BuildingLocation> unvisited = new List<BuildingLocation>();
    List<BuildingLocation> closedSet = new List<BuildingLocation>();
    List<BuildingLocation> openSet = new List<BuildingLocation>();

    public Transform planet;
    public GameObject buildingConstructorPrefab;

    bool initialised = false;
    CharacterSystem characterSystem;

    public delegate void OnBuildingPlaced(BuildingLocation location, BuildingPlacer buildingData, Building building);
    public static OnBuildingPlaced onBuildingPlaced;

    public ClickableBarPopup buildUI;
    bool uiActive = false;
    bool isBuilding = false;
    public BuildingLocation startLocation;
    public BuildingLocation selectedLocation;

    //bool place = false;

    private void Awake()
    {
        foreach (Building building in FindObjectsOfType<Building>())
            building.gameObject.SetActive(false);

        locations = new Dictionary<LocationType, List<BuildingLocation>>();
    }

    private void OnEnable()
    {
        BuildingCloudEffect.onEffectFinish += UpdateConstructors;
    }

    private void OnDisable()
    {
        BuildingCloudEffect.onEffectFinish -= UpdateConstructors;
    }

    private void UpdateConstructors()
    {
        var possibilities = GetPossibleBuildingLocations();

        if (possibilities != null)
            foreach (BuildingLocation location in possibilities)
                EnableLocation(location, true);

        isBuilding = false;
    }

    private void Update()
    {
        if (InputRedirect.tapped && !InputRedirect.inputOverUI)
        {
            InvalidateSelection();

            Ray ray = Camera.main.ScreenPointToRay(InputRedirect.inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Building building = null;

                if (hit.transform.parent != null)
                    building = hit.transform.parent.GetComponent<Building>();

                if (building == null)
                {
                    BuildingConstructor constructor = hit.collider.GetComponent<BuildingConstructor>();
                    if (constructor != null)
                    {
                        if (selectedLocation != null)
                            foreach (BuildingLocation neighbour in selectedLocation.neighbours)
                                selectedLocation.roads[neighbour].gameObject.SetActive(false);

                        selectedLocation = constructor.location;

                        foreach (BuildingLocation neighbour in selectedLocation.neighbours)
                            if (neighbour.state == LocationState.Closed)
                                selectedLocation.roads[neighbour].gameObject.SetActive(true);

                        buildUI.GetType().GetMethod(selectedLocation.locationType.ToString() + "Start").Invoke(buildUI, new object[] { });
                        uiActive = true;
                    }
                }
                else
                {
                    BuildingLocation location = hit.collider.transform.parent.parent.gameObject.GetComponent<BuildingLocation>();
                    if (location != null)
                    {
                        selectedLocation = location;
                        buildUI.DestroyStart(building);
                    }
                }
            }
        }
    }

    private void InvalidateSelection()
    {
        if (selectedLocation != null)
        {
            if (uiActive)
            {
                foreach (BuildingLocation neighbour in selectedLocation.neighbours)
                    selectedLocation.roads[neighbour].gameObject.SetActive(false);

                buildUI.GetType().GetMethod(selectedLocation.locationType.ToString() + "Stop").Invoke(buildUI, new object[] { });
                uiActive = false;
            }
            else
                buildUI.DestroyStop();

            selectedLocation = null;
        }
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
            Gizmos.DrawSphere(location.transform.position, 0.055f);
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
            Gizmos.DrawSphere(location.transform.position, 0.045f);
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
            Gizmos.DrawSphere(location.transform.position, 0.035f);
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
        if (location.state == LocationState.Destroyed)
        {
            foreach (BuildingLocation neighbour in location.neighbours)
                if (neighbour.state == LocationState.Closed)
                {
                    SetLocationState(location, LocationState.Open);
                    return;
                }

            SetLocationState(location, LocationState.Unvisited);
        }
    }

    public void DestroySelectedBuilding()
    {
        if (openSet.Count == 0)
            foreach (BuildingLocation location in unvisited)
                EnableLocation(location, false);

        DestroyBuilding(selectedLocation);
        InvalidateSelection();

        if (openSet.Count == 0)
        {
            foreach (BuildingLocation loc in GetPossibleBuildingLocations())
            {
                EnableLocation(loc, true);
            }
        }
    }

    public void DestroyLocation(BuildingLocation location)
    {
        if (location.state == LocationState.Destroyed)
            return;

        if (DestroyBuilding(location))
            GameManager.buildingsFlooded++;

        SetLocationState(location, LocationState.Destroyed);
    }

    public bool DestroyBuilding(BuildingLocation location)
    {
        if (location.state != LocationState.Closed)
            return false;

        GameManager.buildingsDestroyed++;

        Building building = location.GetComponentInChildren<Building>();
        if (building == null)
            return false;

        bool closedNeighbours = false;
        foreach (BuildingLocation neighbour in location.neighbours)
        {
            if (neighbour.state == LocationState.Open)
            {
                bool closedfurterNeighbours = false;

                foreach (BuildingLocation furterNeighbour in neighbour.neighbours)
                    if (furterNeighbour != location && furterNeighbour.state == LocationState.Closed)
                    {
                        closedfurterNeighbours = true;
                        break;
                    }

                if (!closedfurterNeighbours)
                {
                    SetLocationState(neighbour, LocationState.Unvisited);
                }
            }
            else if (neighbour.state == LocationState.Closed)
                closedNeighbours = true;
        }

        if (closedNeighbours)
        {
            SetLocationState(location, LocationState.Open);
        }
        else
            SetLocationState(location, LocationState.Unvisited);

        for (int i = 0; i < location.roads.Count; i++)
        {
            if (location.roads[i] == null)
            {
                location.roads.RemoveAt(i);
                i--;
                continue;
            }
            location.roads[i].gameObject.SetActive(false);
        }

        GameObject source = location.GetType().GetField(building.buildingType.ToString()).GetValue(location) as GameObject;

        if (source.scene.name == null || source.scene.rootCount == 0)
            Destroy(building.gameObject);
        else
            building.gameObject.SetActive(false);

        characterSystem.DespawnCharacter(location);

        GameManager.AddState(-building.natureRemovalEffect, -building.pollutionRemovalEffect, -building.industryRemovalEffect, - building.happinessRemovalEffect);

        return true;
    }
    #endregion

    #region character path finding
    public bool IsValidTravelLocation(BuildingLocation location)
    {
        return location.state == LocationState.Closed;
    }

    public BuildingLocation GetValidTravelLocation(BuildingLocation exclude = null)
    {
        List<BuildingLocation> possibilities = new List<BuildingLocation>(closedSet);
        if (exclude != null && exclude.state == LocationState.Closed)
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

    private void ConstructAStarPath(ref Queue<BuildingLocation> path, AStarNode current)
    {
        if (current.parent != null)
            ConstructAStarPath(ref path, current.parent);

        path.Enqueue(current.location);
    }

    public Queue<BuildingLocation> GetPath(BuildingLocation start, BuildingLocation target)
    {
        Dictionary<BuildingLocation, AStarNode> nodes = new Dictionary<BuildingLocation, AStarNode>();
        SortedDictionary<float, List<AStarNode>> open = new SortedDictionary<float, List<AStarNode>>();

        foreach (var loc in closedSet)
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
                Queue<BuildingLocation> path = new Queue<BuildingLocation>();
                ConstructAStarPath(ref path, current);
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
                location.state = LocationState.Unvisited;
            }
    }

    private void SetLocationState(BuildingLocation location, LocationState state)
    {
        if (location.state == state)
            return;

        switch (location.state)
        {
            case LocationState.Unvisited:
                unvisited.Remove(location);
                break;
            case LocationState.Open:
                openSet.Remove(location);
                break;
            case LocationState.Closed:
                closedSet.Remove(location);
                break;
            case LocationState.Destroyed:
                destroyed.Remove(location);
                break;
            default:
                break;
        }

        EnableLocation(location, false);
        location.state = state;

        switch (location.state)
        {
            case LocationState.Unvisited:
                unvisited.Add(location);
                break;
            case LocationState.Open:
                openSet.Add(location);
                if (!isBuilding)
                    EnableLocation(location, true);
                break;
            case LocationState.Closed:
                closedSet.Add(location);
                break;
            case LocationState.Destroyed:
                destroyed.Add(location);
                break;
            default:
                break;
        }

        if (closedSet.Count == 0)
            EnableLocation(startLocation, true);
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
    }

    public void EnableLocation(BuildingLocation location, bool enable)
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

        foreach (BuildingLocation location in openSet)
            EnableLocation(location, false);

        if (openSet.Count == 0)
            foreach (BuildingLocation location in unvisited)
                EnableLocation(location, false);

        isBuilding = true;
        ConstructBuilding(selectedLocation, buildingData);
        buildUI.GetType().GetMethod(selectedLocation.locationType.ToString() + "Stop").Invoke(buildUI, new object[] { });
        uiActive = false;
        selectedLocation = null;
    }

    public List<BuildingLocation> GetPossibleBuildingLocations()
    {
        if (openSet.Count == 0)
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

        return openSet;
    }

    private void ConstructBuilding(BuildingLocation location, BuildingPlacer buildingData)
    {
        if (location == null)
            Debug.Log("zafuq?");

        if (buildingData == null)
            Debug.Log("dafuq?");

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
        building.happinessRemovalEffect = buildingData.happinessEffect;

        SetLocationState(location, LocationState.Closed);

        foreach (BuildingLocation neighbour in location.neighbours)
            if (neighbour.state == LocationState.Unvisited)
            {
                SetLocationState(neighbour, LocationState.Open);
            }
            else if (neighbour.state == LocationState.Closed)
            {
                location.roads[neighbour].gameObject.SetActive(true);
            }

        GameManager.AddState(buildingData.natureEffect, buildingData.pollutionEffect, buildingData.industryEffect, buildingData.happinessEffect);
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
