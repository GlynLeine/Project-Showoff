#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    Spline spline;

    float controlSize = 0.05f;
    int selectedSegment = -1;
    bool hideMainTool = true;

    ArcHandle rotationHandle = new ArcHandle();


    private void OnEnable()
    {
        spline = target as Spline;
        if (hideMainTool)
            Tools.hidden = true;
    }

    private void OnDisable()
    {
        if (hideMainTool)
            Tools.hidden = false;
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

        bool newHide = GUILayout.Toggle(hideMainTool, "Hide object handle");
        if (newHide != hideMainTool)
        {
            hideMainTool = newHide;
            Tools.hidden = hideMainTool;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
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
        screenMousePos *= 1.25f;

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

        if (guiEvent.type == EventType.MouseDown && guiEvent.shift)
        {
            if (guiEvent.button == 0)
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
                float mindist = float.MaxValue;
                for (int i = 0; i < spline.PointCount; i += 3)
                {
                    Vector3 anchor = spline.GetWorldPoint(i);
                    ray = sv.camera.ScreenPointToRay(sv.camera.WorldToScreenPoint(anchor));
                    plane.Raycast(ray, out dist);
                    anchor = ray.GetPoint(dist);

                    float distance = Vector2.Distance(mousePos, anchor);
                    if (distance < mindist)
                        mindist = distance;
                    if (distance < controlSize)
                    {
                        Undo.RecordObject(spline, "remove segment");
                        spline.RemoveSegment(i);
                        break;
                    }
                }
            }
        }

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
            Vector3 splinePointWorldPos = spline.GetWorldPoint(i);
            if (Tools.current == Tool.Rotate)
            {
                if (i % 3 != 0)
                    continue;

                Handles.color = Color.white;
                int index = i / 3;
                Vector3 handleDirection = spline.transform.up;
                Vector3 handleNormal = spline.GetWorldForward(index);
                Matrix4x4 handleMatrix = Matrix4x4.TRS(
                    splinePointWorldPos,
                    Quaternion.LookRotation(handleDirection, handleNormal),
                    Vector3.one
                );

                rotationHandle.angle = Vector3.SignedAngle(handleDirection, spline.GetWorldNormal(index), handleNormal) - 90;
                rotationHandle.radius = controlSize;
                using (new Handles.DrawingScope(handleMatrix))
                {
                    // draw the handle
                    EditorGUI.BeginChangeCheck();
                    rotationHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        // record the target object before setting new values so changes can be undone/redone
                        Undo.RecordObject(spline, "rotate point");

                        // copy the handle's updated data back to the target object
                        Vector3 newNormal = Quaternion.AngleAxis(rotationHandle.angle + 90, handleNormal) * handleDirection;
                        spline.SetWorldNormal(index, newNormal);
                    }
                }
            }
            else
            {
                if (i % 3 == 0)
                    Handles.color = Color.red;
                else if (spline.AutoTangents)
                    continue;
                else
                    Handles.color = Color.green;

                Vector3 newWorldPos = Handles.FreeMoveHandle(splinePointWorldPos, Quaternion.identity, controlSize, Vector2.zero, Handles.SphereHandleCap);
                if (splinePointWorldPos != newWorldPos)
                {
                    Undo.RecordObject(spline, "move spline point");
                    spline.MovePoint(i, spline.transform.InverseTransformPoint(newWorldPos));
                }
            }

        }
    }
}

#endif