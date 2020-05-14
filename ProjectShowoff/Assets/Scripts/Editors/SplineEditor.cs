#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    Spline spline;

    float controlSize = 0.05f;
    int selectedSegment = -1;

    private void OnEnable()
    {
        spline = target as Spline;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (GUILayout.Button("Reset"))
        {
            Undo.RecordObject(spline, "reset spline");
            spline.Reset();
        }

        bool autoControls = GUILayout.Toggle(spline.AutoTangents, "Automatically manage tangents");
        if (autoControls != spline.AutoTangents)
        {
            Undo.RecordObject(spline, "toggle auto tangents");
            spline.AutoTangents = autoControls;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
            spline.onSplineUpdate?.Invoke();
        }
    }

    private void OnSceneGUI()
    {
        HandleInput();
        Draw();
    }

    public void HandleInput()
    {
        Event guiEvent = Event.current;

        SceneView sv = SceneView.lastActiveSceneView;

        GUIStyle style = "GV Gizmo DropDown";
        Vector2 ribbon = style.CalcSize(sv.titleContent);

        Vector2 sv_correctSize = sv.position.size;
        sv_correctSize.y -= ribbon.y; //exclude this nasty ribbon

        //flip the position:
        Vector2 screenMousePos = Event.current.mousePosition;
        screenMousePos.y = sv_correctSize.y - screenMousePos.y;
        //screenMousePos *= 1.25f;

        Ray ray = sv.camera.ScreenPointToRay(new Vector3(screenMousePos.x, screenMousePos.y, sv.camera.nearClipPlane));
        Plane plane = new Plane(spline.transform.forward, spline.transform.position);
        plane.Raycast(ray, out float dist);
        Vector3 mousePos = ray.GetPoint(dist);
        Vector3 relativeMousepos = spline.transform.InverseTransformPoint(mousePos);

        if (guiEvent.type == EventType.MouseMove)
        {
            int newSelectedSegment = -1;
            float minDist = float.MaxValue;
            for (int i = 0; i < spline.SegmentCount; i++)
            {
                Vector3[] points = spline.GetWorldPointsInSegment(i);

                for (int j = 0; j < 4; j++)
                {
                    ray = sv.camera.ScreenPointToRay(sv.camera.WorldToScreenPoint(points[j]));
                    plane.Raycast(ray, out dist);
                    points[j] = ray.GetPoint(dist);
                }

                float distance = HandleUtility.DistancePointBezier(mousePos,
                     points[0],
                     points[3],
                     points[1],
                     points[2]);

                if (distance < minDist)
                    minDist = distance;

                if (distance <= controlSize * 2)
                {
                    newSelectedSegment = i;
                    break;
                }
            }

            if (newSelectedSegment != selectedSegment)
            {
                selectedSegment = newSelectedSegment;
                HandleUtility.Repaint();
            }
        }

        if (guiEvent.type == EventType.MouseDown)
        {
            if (guiEvent.button == 0 && guiEvent.shift)
            {
                if (selectedSegment >= 0 && selectedSegment < spline.SegmentCount)
                {
                    Undo.RecordObject(spline, "split segment");
                    spline.SplitSegment(relativeMousepos, selectedSegment);
                }
                else
                {
                    Undo.RecordObject(spline, "add segment");
                    spline.AddSegment(relativeMousepos);
                }
            }
            else if (guiEvent.button == 1)
            {
                for (int i = 0; i < spline.PointCount; i += 3)
                {
                    float distance = Vector2.Distance(relativeMousepos, spline[i]);
                    if (distance < controlSize * 0.5f)
                    {
                        Undo.RecordObject(spline, "remove segment");
                        spline.RemoveSegment(i);
                        break;
                    }
                }
            }
        }

        //Handles.SphereHandleCap(0, mousePos, Quaternion.identity, controlSize, guiEvent.type);
        HandleUtility.AddDefaultControl(0);
    }

    public void Draw()
    {
        Handles.color = Color.blue;
        for (int i = 0; i < spline.SegmentCount; i++)
        {
            Vector3[] points = spline.GetWorldPointsInSegment(i);
            if (!spline.AutoTangents)
            {
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
            }
            Handles.DrawBezier(points[0], points[3], points[1], points[2], (selectedSegment == i) ? Color.green : Color.white, null, 2);
        }

        for (int i = 0; i < spline.PointCount; i++)
        {
            if (i % 3 == 0)
                Handles.color = Color.red;
            else if (spline.AutoTangents)
                continue;
            else
                Handles.color = Color.green;

            Vector3 splinePointWorldPos = spline.GetWorldPoint(i);
            Vector3 newWorldPos = Handles.FreeMoveHandle(splinePointWorldPos, Quaternion.identity, controlSize, Vector2.zero, Handles.SphereHandleCap);
            if (splinePointWorldPos != newWorldPos)
            {
                Undo.RecordObject(spline, "move spline point");
                spline.MovePoint(i, spline.transform.InverseTransformPoint(newWorldPos));
            }

        }
    }
}

#endif