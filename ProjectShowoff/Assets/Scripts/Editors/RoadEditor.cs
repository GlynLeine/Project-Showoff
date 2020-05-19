#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
[CanEditMultipleObjects]
public class RoadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if(Event.current.type == EventType.Repaint)
            (target as Road).Validate();
    }
}

#endif