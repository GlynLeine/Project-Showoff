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

    public void Validate()
    {
        if (spline == null)
            spline = GetComponent<Spline>();
        if (start != null)
            spline.MovePoint(0, transform.InverseTransformPoint(start.transform.position));
        if (end != null)
            spline.MovePoint(spline.PointCount - 1, transform.InverseTransformPoint(end.transform.position));

        GetComponent<SplineMesh>().UpdateMesh();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        GameObject parent = GameObject.Find("/Planet/Roads");
        if(parent == null)
        {
            parent = new GameObject("Roads");
            parent.transform.parent = GameObject.Find("/Planet").transform;
        }
        transform.parent = parent.transform;
    }
}
