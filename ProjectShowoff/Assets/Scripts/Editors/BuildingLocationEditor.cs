#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(BuildingLocation))]
[CanEditMultipleObjects]
public class BuildingLocationEditor : Editor
{
    BuildingLocation location;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void OnEnable()
    {
        //Tools.hidden = true;

        if (location == null)
            location = target as BuildingLocation;

    }

    private void OnDisable()
    {
        //Tools.hidden = false;
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
    }
}

#endif