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
                GameObject gameObject = new GameObject(name + "location");
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
                    neighbour.neighbours.Remove(selected);

                DestroyImmediate(selected.gameObject);
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
            BuildingLocation[] locations = FindObjectsOfType<BuildingLocation>();
            for (int i = 0; i < locations.Length; i++)
                locations[i].gameObject.AddComponent<SphereCollider>();

            for (int i = 0; i < locations.Length; i++)
            {
                foreach (Transform child in locations[i].transform)
                    DestroyImmediate(child.gameObject);
            }

        }
    }

    private void OnDisable()
    {
        Tools.hidden = false;

        if (tool.planet != null && tool != null)
        {
            BuildingLocation[] locations = FindObjectsOfType<BuildingLocation>();
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
            BuildingLocation[] locations = FindObjectsOfType<BuildingLocation>();
            for (int i = 0; i < locations.Length; i++)
            {
                Vector3 rayPos = Camera.current.transform.position;
                Vector3 rayDir = (locations[i].transform.position - rayPos).normalized;

                SphereCollider collider = locations[i].GetComponent<SphereCollider>();
                collider.radius = HandleUtility.GetHandleSize(locations[i].transform.position) * 0.1f;

                if (Event.current.type == EventType.MouseDown && Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out RaycastHit hit))
                    if (hit.collider == collider)
                        if (neighbourMode)
                        {
                            if (selected != locations[i])
                            {
                                bool has = selected.neighbours.Contains(locations[i]);
                                Undo.RecordObject(selected, has ? "remove neighbour" : "add neighbour");

                                if (has)
                                {
                                    selected.neighbours.Remove(locations[i]);
                                    if (locations[i].neighbours.Contains(selected))
                                        locations[i].neighbours.Remove(selected);

                                    Repaint();
                                }
                                else
                                {
                                    selected.neighbours.Add(locations[i]);
                                    if (!locations[i].neighbours.Contains(selected))
                                        locations[i].neighbours.Add(selected);

                                    Repaint();
                                }

                                EditorUtility.SetDirty(selected);
                                EditorSceneManager.MarkSceneDirty(tool.gameObject.scene);
                            }
                        }
                        else
                        {
                            selected = locations[i];
                            Repaint();
                        }

                if (Physics.Raycast(rayPos, rayDir, out hit))
                {
                    if (hit.collider != collider)
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

    private void DrawRoadmap(BuildingLocation location)
    {
        for (int i = 0; i < location.neighbours.Count; i++)
            Handles.DrawLine(location.neighbours[i].transform.position, location.transform.position);
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
            location.transform.position = normal * tool.surface;

            EditorUtility.SetDirty(location.transform);
            EditorSceneManager.MarkSceneDirty(tool.gameObject.scene);
        }
    }
}

#endif