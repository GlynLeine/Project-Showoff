#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CloudEffect))]
public class CloudEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CloudEffect cloudEffect = target as CloudEffect;

        if (GUILayout.Button("Generate clouds"))
        {
            while (cloudEffect.transform.childCount > 0)
            {
                DestroyImmediate(cloudEffect.transform.GetChild(0).gameObject);
            }

            cloudEffect.Start();
        }

        if (GUILayout.Button("Clean up"))
        {
            while(cloudEffect.transform.childCount > 0)
            {
                DestroyImmediate(cloudEffect.transform.GetChild(0).gameObject);
            }
        }
    }
}

#endif