#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    Road road;

    private void OnEnable()
    {
        road = target as Road;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if(Event.current.type == EventType.Repaint)
            road.OnValidate();
    }
}

#endif