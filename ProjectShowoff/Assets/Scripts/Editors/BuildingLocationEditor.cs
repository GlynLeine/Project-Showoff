#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(BuildingLocation))]
[CanEditMultipleObjects]
public class BuildingLocationEditor : Editor
{
    BuildingLocation location;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            System.Type type = location.GetType();

            foreach (BuildingType buildingType in System.Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>())
            {
                string buildingTypeName = buildingType.ToString();
                var field = type.GetField(buildingTypeName);

                GameObject value = field.GetValue(location) as GameObject;
                if (value == null)
                {
                    var guids = AssetDatabase.FindAssets(buildingTypeName, new string[] { "Assets/Prefabs" });

                    field.SetValue(location, AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])));
                }
            }
        }
    }

    private void OnEnable()
    {
        if (location == null)
            location = target as BuildingLocation;

        if (location != null)
        {
            System.Type type = location.GetType();

            foreach (BuildingType buildingType in System.Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>())
            {
                string buildingTypeName = buildingType.ToString();
                var field = type.GetField(buildingTypeName);

                GameObject value = field.GetValue(location) as GameObject;
                if (value == null)
                {
                    var guids = AssetDatabase.FindAssets(buildingTypeName, new string[] { "Assets/Prefabs" });

                    field.SetValue(location, AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])));

                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    EditorUtility.SetDirty(location);
                }
            }
        }

    }

    private void OnDisable()
    {
        if (location == null)
            location = target as BuildingLocation;

        if (location != null)
        {
            System.Type type = location.GetType();

            foreach (BuildingType buildingType in System.Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>())
            {
                string buildingTypeName = buildingType.ToString();
                var field = type.GetField(buildingTypeName);

                GameObject value = field.GetValue(location) as GameObject;
                if (value == null)
                {
                    var guids = AssetDatabase.FindAssets(buildingTypeName, new string[] { "Assets/Prefabs" });

                    field.SetValue(location, AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0])));
                }
            }
        }
    }

    private void OnSceneGUI()
    {
        int i = 0;
        foreach (BuildingLocation neighbour in location.neighbours)
        {
            i++;
            float r = Mathf.Clamp(i * 0.5f, 0, 1f);
            float g = Mathf.Clamp(i * 0.5f - 1, 0, 1f);
            if (g > 0)
                r = 0;
            float b = Mathf.Clamp(i * 0.5f - 2, 0, 1f);
            if (b > 0)
                g = 0;
            Handles.color = new Color(r, g, b);
            Handles.DrawLine(neighbour.transform.position, location.transform.position);
        }

        BuildingLocationTool tool = FindObjectOfType<BuildingLocationTool>();
        if (tool != null)
        {
            Vector3 normal = (location.transform.position - tool.planet.position).normalized;
            location.transform.up = normal;
        }
    }
}

#endif