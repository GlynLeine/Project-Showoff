using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineMesh))]
public class Road : MonoBehaviour
{
    [HideInInspector]
    public Spline spline;

    public BuildingLocation start;
    public BuildingLocation end;

    public void OnValidate()
    {
        if (spline == null)
            spline = GetComponent<Spline>();
        if (start != null)
            spline.MovePoint(0, transform.InverseTransformPoint(start.transform.position));
        if (end != null)
            spline.MovePoint(spline.PointCount - 1, transform.InverseTransformPoint(end.transform.position));
    }
}
