#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineMesh))]
[CanEditMultipleObjects]
public class SplineMeshEditor : Editor
{
    private void OnEnable()
    {
        (target as SplineMesh).UpdateMesh();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
            (target as SplineMesh).UpdateMesh();
    }

    private void OnSceneGUI()
    {
        if(Event.current.type == EventType.Repaint)
            (target as SplineMesh).UpdateMesh();
    }
}

#endif