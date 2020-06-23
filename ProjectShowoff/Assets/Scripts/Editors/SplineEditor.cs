#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{
    Spline spline;

    float controlSize = 0.3f;
    int selectedSegment = -1;
    static bool hideMainTool;
    static bool showVertexPath;
    static bool disableOtherSelection;
    static bool alternateMousePos;

    ArcHandle rotationHandle = new ArcHandle();


    private void OnEnable()
    {
        spline = target as Spline;
        if (hideMainTool)
            Tools.hidden = true;

        disableOtherSelection = false;
        alternateMousePos = false;
        showVertexPath = false;
        hideMainTool = true;
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

        GUI.enabled = false;
        EditorGUILayout.FloatField("Length", spline.length);
        GUI.enabled = true;

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

        bool isClosed = GUILayout.Toggle(spline.Closed, "Close spline to loop");
        if (isClosed != spline.Closed)
        {
            Undo.RecordObject(spline, "toggle spline closed");
            spline.Closed = isClosed;
        }

        bool newHide = GUILayout.Toggle(hideMainTool, "Hide object handle");
        if (newHide != hideMainTool)
        {
            hideMainTool = newHide;
            Tools.hidden = hideMainTool;
        }

        showVertexPath = GUILayout.Toggle(showVertexPath, "Show vertex path");

        disableOtherSelection = GUILayout.Toggle(disableOtherSelection, "Lock selection on this object");

        alternateMousePos = GUILayout.Toggle(alternateMousePos, "Unity mouse position glitch fix");

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
            Repaint();
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
        if (alternateMousePos)
            screenMousePos *= 1.25f;

        Ray ray = sv.camera.ScreenPointToRay(new Vector3(screenMousePos.x, screenMousePos.y, sv.camera.nearClipPlane));

        Vector3 splineForward = (spline.GetWorldPoint(spline.PointCount - 1) - spline.GetWorldPoint(0)).normalized;
        Vector3 splineRight = Vector3.Slerp(spline.GetWorldNormal(0), spline.GetWorldNormal(spline.SegmentCount), 0.5f).normalized;

        Plane plane = new Plane(Vector3.Cross(splineForward, splineRight).normalized, spline.GetWorldPoint(0));
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

                if (distance <= controlSize * 0.2f)
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
                    Vector3 worldPos = spline.GetWorldPoint(i);
                    Vector3 anchor = worldPos;
                    ray = sv.camera.ScreenPointToRay(sv.camera.WorldToScreenPoint(anchor));
                    plane.Raycast(ray, out dist);
                    anchor = ray.GetPoint(dist);

                    float distance = Vector2.Distance(mousePos, anchor);
                    if (distance < mindist)
                        mindist = distance;
                    if (distance < controlSize * HandleUtility.GetHandleSize(worldPos))
                    {
                        Undo.RecordObject(spline, "remove segment");
                        spline.RemoveSegment(i);
                        break;
                    }
                }
            }
        }

        if (disableOtherSelection)
            HandleUtility.AddDefaultControl(0);
    }

    public void Draw()
    {
        Handles.color = Color.blue;
        for (int i = 0; i < spline.SegmentCount; i++)
        {
            Vector3[] points = spline.GetWorldPointsInSegment(i);
            if (!spline.AutoTangents && Tools.current != Tool.Rotate)
            {
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
            }
            Handles.DrawBezier(points[0], points[3], points[1], points[2], (selectedSegment == i) ? Color.green : Color.white, null, 2);
        }


        if (spline.VertexPath.valid && showVertexPath)
        {
            if (Event.current.type == EventType.Repaint)
                for (int i = 0; i < spline.VertexPath.VertexCount; i++)
                {
                    Vector3 vertex = spline.transform.TransformPoint(spline.VertexPath[i]);
                    Vector3 forward = spline.transform.TransformDirection(spline.VertexPath.GetTangent(i));
                    Vector3 right = spline.transform.TransformDirection(spline.VertexPath.GetNormal(i));
                    Vector3 up = Vector3.Cross(forward, right).normalized;

                    Handles.color = Color.blue;
                    Handles.ArrowHandleCap(0, vertex, Quaternion.LookRotation(forward, up), controlSize * HandleUtility.GetHandleSize(vertex), EventType.Repaint);

                    Handles.color = Color.red;
                    Handles.ArrowHandleCap(0, vertex, Quaternion.LookRotation(right, up), controlSize * HandleUtility.GetHandleSize(vertex), EventType.Repaint);

                    Handles.color = Color.green;
                    Handles.ArrowHandleCap(0, vertex, Quaternion.LookRotation(up, forward), controlSize * HandleUtility.GetHandleSize(vertex), EventType.Repaint);
                }
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

                Vector3 oldNormal = spline.GetWorldNormal(index);
                Vector3 handleNormal = spline.GetWorldForward(i);
                Vector3 handleDirection = Vector3.Cross(oldNormal, handleNormal);

                Matrix4x4 handleMatrix = Matrix4x4.TRS(
                    splinePointWorldPos,
                    Quaternion.LookRotation(handleDirection, handleNormal),
                    Vector3.one
                );

                rotationHandle.angle = Vector3.SignedAngle(handleDirection, spline.GetWorldNormal(index), handleNormal) - 90;
                rotationHandle.radius = controlSize * HandleUtility.GetHandleSize(splinePointWorldPos) * 2;
                using (new Handles.DrawingScope(handleMatrix))
                {
                    EditorGUI.BeginChangeCheck();
                    rotationHandle.DrawHandle();
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(spline, "rotate point");

                        Vector3 newNormal = Quaternion.AngleAxis(rotationHandle.angle + 90, handleNormal) * handleDirection;
                        spline.SetWorldNormal(index, newNormal);
                        Repaint();
                    }
                }
            }
            else
            {
                Vector3 newWorldPos = Vector3.zero;

                if (i % 3 == 0)
                {
                    Handles.color = Color.red;
                    if (Tools.pivotRotation == PivotRotation.Local)
                    {
                        Vector3 forward = spline.GetWorldForward(i);
                        Vector3 up = Vector3.Cross(spline.GetWorldNormal(i / 3), forward).normalized;
                        newWorldPos = Handles.PositionHandle(splinePointWorldPos, Quaternion.LookRotation(forward, up));
                    }
                    else
                        newWorldPos = Handles.PositionHandle(splinePointWorldPos, Quaternion.identity);
                }
                else if (spline.AutoTangents)
                    continue;
                else
                {
                    Handles.color = Color.green;
                    newWorldPos = Handles.FreeMoveHandle(splinePointWorldPos, Quaternion.identity, controlSize * HandleUtility.GetHandleSize(splinePointWorldPos), Vector2.zero, Handles.SphereHandleCap);
                }

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