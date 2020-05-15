using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineMesh))]
public class Road : MonoBehaviour
{
    [HideInInspector]
    public Spline spline;

    //public void CreateSpline()
    //{
    //    spline = new Spline(transform.position);
    //}

    //private void Reset()
    //{
    //    CreateSpline();
    //}
}
