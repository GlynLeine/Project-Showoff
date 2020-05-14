using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spline))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
class SplineMesh : MonoBehaviour
{
    [HideInInspector]
    public Spline spline = null;

    [HideInInspector]
    public Mesh mesh = null;

    public bool flattenSurface;
    [Range(0.01f, 10f)]
    public float meshWidth = 1;
   // [Range(0, .5f)]
    public float thickness = .15f;

    public void Reset()
    {
        spline = null;
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        if (spline == null)
        {
            spline = GetComponent<Spline>();
        }

        spline.onSplineUpdate -= UpdateMesh;
        spline.onSplineUpdate += UpdateMesh;

        CreateRoadMesh(spline.VertexPath);
    }

    void CreateRoadMesh(VertexPath path)
    {
        Vector3[] verts = new Vector3[path.VertexCount * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (path.VertexCount - 1);
        int[] roadTriangles = new int[numTris * 3];
        int[] underRoadTriangles = new int[numTris * 3];
        int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 8  9
        // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        bool usePathNormals = !flattenSurface;

        for (int i = 0; i < path.VertexCount; i++)
        {
            Vector3 localUp = usePathNormals ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
            Vector3 localRight = usePathNormals ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

            // Find position to left and right of current path vertex
            Vector3 vertSideA = path[i] - localRight * Mathf.Abs(meshWidth);
            Vector3 vertSideB = path[i] + localRight * Mathf.Abs(meshWidth);

            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA;
            verts[vertIndex + 1] = vertSideB;
            // Add bottom of road vertices
            verts[vertIndex + 2] = vertSideA - localUp * thickness;
            verts[vertIndex + 3] = vertSideB - localUp * thickness;

            // Duplicate vertices to get flat shading for sides of road
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2(0, path.GetTime(i));
            uvs[vertIndex + 1] = new Vector2(1, path.GetTime(i));

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of road normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of road normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Set triangle indices
            if (i < path.VertexCount - 1)
            {
                for (int j = 0; j < triangleMap.Length; j++)
                {
                    roadTriangles[triIndex + j] = vertIndex + triangleMap[j];
                    // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                    underRoadTriangles[triIndex + j] = vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++)
                {
                    sideOfRoadTriangles[triIndex * 2 + j] = vertIndex + sidesTriangleMap[j];
                }

            }

            vertIndex += 8;
            triIndex += 6;
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Spline Mesh";
        }
        else
            mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles(roadTriangles, 0);
        mesh.SetTriangles(underRoadTriangles, 1);
        mesh.SetTriangles(sideOfRoadTriangles, 2);
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
