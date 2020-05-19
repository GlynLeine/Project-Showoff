#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(BuildingLocationTool))]
[CanEditMultipleObjects]
public class BuildingLocationToolEditor : Editor
{
    BuildingLocationTool tool;

    static bool alwaysShow = false;

    BuildingLocation selected;
    bool neighbourMode = false;
    bool viewRoadmap = false;
    Camera camera;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        foreach (string name in System.Enum.GetNames(typeof(LocationType)))
            if (GUILayout.Button("Add new " + name + " location"))
            {
                GameObject gameObject = new GameObject(name + " location " + tool.transform.GetComponentsInChildren<BuildingLocation>().Length);
                gameObject.transform.parent = tool.transform;
                BuildingLocation location = gameObject.AddComponent<BuildingLocation>();
                location.locationType = (LocationType)System.Enum.Parse(typeof(LocationType), name);
                location.gameObject.AddComponent<SphereCollider>();

                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit))
                    gameObject.transform.position = hit.point;

                SceneView.RepaintAll();
            }

        if (selected != null)
            if (GUILayout.Button("Destroy selected location"))
            {
                foreach (BuildingLocation neighbour in selected.neighbours)
                {
                    neighbour.neighbours.Remove(selected);
                    DestroyImmediate(neighbour.roads[selected].gameObject);
                    neighbour.roads.Remove(selected);
                }

                DestroyImmediate(selected.gameObject);
                selected = null;
                neighbourMode = false;
                SceneView.RepaintAll();
            }

        if (GUILayout.Button(viewRoadmap ? "Stop viewing roadmap" : "View roadmap"))
        {
            viewRoadmap = !viewRoadmap;
            SceneView.RepaintAll();
        }

        if (GUILayout.Button(alwaysShow ? "Stop always showing locations" : "Always show locations"))
        {
            alwaysShow = !alwaysShow;
        }

        if (selected != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected location: " + selected.gameObject.name);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button(neighbourMode ? "Stop changing neighbours" : "Change neighbours"))
            {
                neighbourMode = !neighbourMode;
                SceneView.RepaintAll();
            }

            Editor locationEditor = CreateEditor(selected);
            locationEditor.OnInspectorGUI();

            if (GUILayout.Button("Go to selected"))
            {
                Selection.objects = new Object[] { selected.gameObject };
            }
        }
    }

    private void OnEnable()
    {
        Tools.hidden = true;

        if (tool == null)
            tool = target as BuildingLocationTool;

        if (tool.planet != null && tool != null)
        {
            BuildingLocation[] locations = tool.transform.GetComponentsInChildren<BuildingLocation>();
            for (int i = 0; i < locations.Length; i++)
                locations[i].gameObject.AddComponent<SphereCollider>();

            for (int i = 0; i < locations.Length; i++)
            {
                foreach (Transform child in locations[i].transform)
                    DestroyImmediate(child.gameObject);
            }
        }

        Validate();
    }

    private void Validate()
    {
        BuildingLocation[] locations = tool.transform.GetComponentsInChildren<BuildingLocation>();
        for (int i = 0; i < locations.Length; i++)
        {
            BuildingLocation location = locations[i];
            for (int j = 0; j < location.neighbours.Count; j++)
            {
                BuildingLocation neighbour = location.neighbours[j];

                if (neighbour == null)
                {
                    location.neighbours.Remove(neighbour);
                    location.roads.Remove(neighbour);
                    j--;
                    continue;
                }

                if (!location.roads.ContainsKey(neighbour))
                    location.roads.Add(neighbour, null);

                if (location.roads[neighbour] == null)
                {
                    if (!neighbour.roads.ContainsKey(location))
                        neighbour.roads.Add(location, null);

                    if (neighbour.roads[location] == null)
                    {
                        AddRoad(location, neighbour);
                    }
                    else
                    {
                        location.roads[neighbour] = neighbour.roads[location];
                        if (location.roads[neighbour].start == neighbour)
                            location.roads[neighbour].end = location;
                        else
                            location.roads[neighbour].start = location;
                    }
                }
            }
        }
    }

    private void AddRoad(BuildingLocation location, BuildingLocation neighbour)
    {
        GameObject roadObject = new GameObject("road " + tool.transform.GetComponentsInChildren<Road>().Length);
        roadObject.transform.parent = tool.transform;
        Spline spline = roadObject.AddComponent<Spline>();
        spline.sphericalNormalInterpolation = true;
        spline.AutoTangents = true;
        spline.resolution = 5;

        SplineMesh splineMesh = roadObject.AddComponent<SplineMesh>();
        splineMesh.meshWidth = 0.025f;
        splineMesh.thickness = 0.01f;

        MeshRenderer meshRenderer = roadObject.GetComponent<MeshRenderer>();
        string guid = AssetDatabase.FindAssets("RoadMat t:material", null)[0];
        meshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));

        Road road = roadObject.AddComponent<Road>();
        road.start = location;
        road.end = neighbour;
        road.Validate();
        spline.AutoTangents = false;
        spline.AutoTangents = true;

        Vector3 middle = tool.planet.position + Vector3.Slerp(location.transform.position - tool.planet.position, neighbour.transform.position - tool.planet.position, 0.5f);

        spline.SplitSegment(spline.transform.InverseTransformPoint(middle), 0);

        Vector3 startUp = (location.transform.position - tool.planet.position).normalized;
        Vector3 middleUp = (middle - tool.planet.position).normalized;
        Vector3 endUp = (neighbour.transform.position - tool.planet.position).normalized;
        Vector3 forward = (location.transform.position - neighbour.transform.position).normalized;

        spline.SetWorldNormal(0, Vector3.Cross(forward, startUp));
        spline.SetWorldNormal(1, Vector3.Cross(forward, middleUp));
        spline.SetWorldNormal(2, Vector3.Cross(forward, endUp));

        splineMesh.UpdateMesh();

        if (!location.roads.ContainsKey(neighbour))
            location.roads.Add(neighbour, null);

        if (!neighbour.roads.ContainsKey(location))
            neighbour.roads.Add(location, null);

        location.roads[neighbour] = road;
        neighbour.roads[location] = road;
    }

    private void OnDisable()
    {
        Tools.hidden = false;

        if (tool.planet != null && tool != null)
        {
            BuildingLocation[] locations = tool.transform.GetComponentsInChildren<BuildingLocation>();
            for (int i = 0; i < locations.Length; i++)
            {
                foreach (SphereCollider collider in locations[i].gameObject.GetComponents<SphereCollider>())
                    DestroyImmediate(collider);
            }

            if (alwaysShow)
            {
                for (int i = 0; i < locations.Length; i++)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.parent = locations[i].transform;
                    sphere.transform.localPosition = Vector3.zero;
                    sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                }
            }
        }
    }

    private void OnSceneGUI()
    {
        if (camera == null)
            camera = Camera.current;

        if (tool.planet != null && tool != null)
        {
            BuildingLocation[] locations = tool.transform.GetComponentsInChildren<BuildingLocation>();
            for (int i = 0; i < locations.Length; i++)
            {
                Vector3 rayPos = Camera.current.transform.position;
                Vector3 rayDir = (locations[i].transform.position - rayPos).normalized;

                HandleInput(locations[i]);

                if (Physics.Raycast(rayPos, rayDir, out RaycastHit hit))
                {
                    if (hit.collider != locations[i].GetComponent<SphereCollider>())
                        continue;
                }
                else
                    continue;

                if (neighbourMode)
                    DrawNeighbourModeSceneHandles(locations[i]);
                else
                {
                    if (viewRoadmap)
                        DrawRoadmap(locations[i]);

                    DrawSceneHandles(locations[i]);
                }
            }
        }
    }

    private void HandleInput(BuildingLocation location)
    {
        SphereCollider collider = location.GetComponent<SphereCollider>();
        collider.radius = HandleUtility.GetHandleSize(location.transform.position) * 0.1f;

        if (Event.current.type == EventType.MouseDown && Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hit))
            if (hit.collider == collider)
                if (neighbourMode)
                {
                    if (selected != location)
                    {
                        bool has = selected.neighbours.Contains(location);
                        Undo.RecordObject(selected, has ? "remove neighbour" : "add neighbour");

                        if (has)
                        {
                            selected.neighbours.Remove(location);
                            selected.roads.Remove(location);

                            if (location.neighbours.Contains(selected))
                            {
                                DestroyImmediate(location.roads[selected].gameObject);
                                location.roads.Remove(selected);
                                location.neighbours.Remove(selected);
                            }
                            Repaint();
                        }
                        else
                        {
                            selected.neighbours.Add(location);
                            if (!location.neighbours.Contains(selected))
                                location.neighbours.Add(selected);

                            AddRoad(selected, location);

                            Repaint();
                        }

                        EditorUtility.SetDirty(selected);
                        EditorSceneManager.MarkSceneDirty(tool.gameObject.scene);
                    }
                }
                else
                {
                    selected = location;
                    Repaint();
                }
    }

    private void DrawRoadmap(BuildingLocation location)
    {

        foreach (BuildingLocation neighbour in location.neighbours)
        {
            if (neighbour == null)
            {
                location.neighbours.Remove(neighbour);
                Debug.LogWarning("something went wrong somewhere...");
            }
            Handles.DrawLine(neighbour.transform.position, location.transform.position);
        }
    }

    private void DrawNeighbourModeSceneHandles(BuildingLocation location)
    {
        Color prevColor = Handles.color;

        if (selected == location)
            Handles.color = new Color(1, 1, 1);
        else if (selected.neighbours.Contains(location))
            Handles.color = new Color(0, 1, 0);
        else
            Handles.color = new Color(0, 0, 1);

        Handles.SphereHandleCap(0, location.transform.position, Quaternion.identity, HandleUtility.GetHandleSize(location.transform.position) * 0.2f, Event.current.type);
        Handles.color = prevColor;

        if (selected.neighbours.Contains(location))
            Handles.DrawLine(selected.transform.position, location.transform.position);
    }

    private void DrawSceneHandles(BuildingLocation location)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newPos = Handles.PositionHandle(location.transform.position, location.transform.rotation);

        if (Event.current.type == EventType.Repaint)
        {
            Color prevColor = Handles.color;
            if (selected == location)
                Handles.color = new Color(1, 1, 1);
            else
                Handles.color = new Color(0, 1 - (int)location.locationType, (int)location.locationType);
            Handles.SphereHandleCap(0, newPos, Quaternion.identity, HandleUtility.GetHandleSize(newPos) * 0.2f, Event.current.type);
            Handles.color = prevColor;
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(location.transform, "position of building location");
            Vector3 normal = (newPos - tool.planet.position).normalized;
            location.transform.up = normal;
            location.transform.position = newPos;

            for (int i = 0; i < location.roads.Count; i++)
                location.roads[i].Validate();

            EditorUtility.SetDirty(location.transform);
            EditorSceneManager.MarkSceneDirty(tool.gameObject.scene);
        }
    }
}

#endif