#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineMesh))]
public class SplineMeshEditor : Editor
{
    SplineMesh splineMesh;

    private void OnEnable()
    {
        splineMesh = target as SplineMesh;
        splineMesh.UpdateMesh();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
            splineMesh.UpdateMesh();
    }
}

#endif