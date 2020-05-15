using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct VertexPath
{
    Vector3[] vertices;
    Vector3[] tangents;
    Vector3[] normals;
    float[] distances;

    public bool valid {get; private set; }

    public float length { get; private set; }

    public int VertexCount => vertices.Length;

    public Vector3 this[int index] => vertices[index];

    public Vector3 up { get; private set; }

    public void UpdatePath(Spline spline)
    {
        valid = true;
        up = spline.transform.up;

        SplineVertexData vertexData = spline.CalculateEvenlySpacedPoints(1f / spline.resolution * 0.5f);

        vertices = vertexData.vertices;
        normals = vertexData.normals;
        tangents = vertexData.tangents;
        distances = vertexData.distances;
        length = vertexData.length;
    }

    public Vector3 GetTangent(int index)
    {
        return tangents[index];
    }

    public Vector3 GetNormal(int index)
    {
        return normals[index];
    }

    public float GetTime(int index)
    {
        return distances[index] / length;
    }

    public float GetDistance(int index)
    {
        return distances[index];
    }

}

