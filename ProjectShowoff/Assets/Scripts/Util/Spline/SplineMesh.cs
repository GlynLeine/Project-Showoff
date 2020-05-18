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
    [Range(0, .5f)]
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

        CreateMesh(spline.VertexPath);
    }
    class Triangle
    {
        public Vector3[] verts = new Vector3[3];
        public Vector2[] uvs = new Vector2[3];
        public Vector3[] normals = new Vector3[3];
    }

    void CreateMesh(VertexPath path)
    {
        List<Triangle> triangles = new List<Triangle>();
        bool usePathNormals = !flattenSurface;

        int[] triangleMap = { 0, 4, 1,/**/ 1, 4, 5,
            /**/ 0, 2, 6,/**/ 0, 6, 4,
            /**/ 2, 3, 6,/**/ 3, 7, 6,
            /**/ 1, 7, 3,/**/ 1, 5, 7 };

        for (int i = 0; i < path.VertexCount - 1; i++)
        {
            Vector3[] verts = new Vector3[8];
            Vector2[] uvs = new Vector2[8];

            Vector3 currentUp = usePathNormals ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : Vector3.up;
            Vector3 currentRight = usePathNormals ? path.GetNormal(i) : Vector3.Cross(currentUp, path.GetTangent(i));

            Vector3 nextUp = usePathNormals ? Vector3.Cross(path.GetTangent(i + 1), path.GetNormal(i + 1)) : Vector3.up;
            Vector3 nextRight = usePathNormals ? path.GetNormal(i + 1) : Vector3.Cross(nextUp, path.GetTangent(i + 1));

            // Find position to left and right of current path vertex
            Vector3 vertSideA = path[i] - currentRight * Mathf.Abs(meshWidth);
            Vector3 vertSideB = path[i] + currentRight * Mathf.Abs(meshWidth);

            Vector3 vertSideC = path[i + 1] - nextRight * Mathf.Abs(meshWidth);
            Vector3 vertSideD = path[i + 1] + nextRight * Mathf.Abs(meshWidth);

            verts[0] = vertSideA;
            verts[1] = vertSideB;
            verts[2] = vertSideA - currentUp * thickness;
            verts[3] = vertSideB - currentUp * thickness;

            uvs[0] = new Vector2(0, path.GetTime(i));
            uvs[1] = new Vector2(1, path.GetTime(i));
            uvs[2] = new Vector2(1, path.GetTime(i));
            uvs[3] = new Vector2(0, path.GetTime(i));

            verts[4] = vertSideC;
            verts[5] = vertSideD;
            verts[6] = vertSideC - nextUp * thickness;
            verts[7] = vertSideD - nextUp * thickness;

            uvs[4] = new Vector2(0, path.GetTime(i + 1));
            uvs[5] = new Vector2(1, path.GetTime(i + 1));
            uvs[6] = new Vector2(1, path.GetTime(i + 1));
            uvs[7] = new Vector2(0, path.GetTime(i + 1));

            for (int j = 0; j < triangleMap.Length; j += 3)
            {
                Triangle triangle = new Triangle();
                for (int k = 0; k < 3; k++)
                {
                    triangle.verts[k] = verts[triangleMap[j + k]];
                    triangle.uvs[k] = uvs[triangleMap[j + k]];
                }


                Vector3 a = triangle.verts[0];
                Vector3 b = triangle.verts[1];
                Vector3 c = triangle.verts[2];

                Vector3 tangent = (b - a).normalized;
                Vector3 bitangent = (c - a).normalized;
                Vector3 normal = Vector3.Cross(tangent, bitangent).normalized;

                triangle.normals[0] = normal;
                triangle.normals[1] = normal;
                triangle.normals[2] = normal;

                triangles.Add(triangle);
            }
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        for (int i = 0; i < triangles.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                vertices.Add(triangles[i].verts[j]);
                uv.Add(triangles[i].uvs[j]);
                normals.Add(triangles[i].normals[j]);
                indices.Add(i * 3 + j);
            }
        }

        mesh = new Mesh();
        mesh.name = "Spline Mesh";

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.normals = normals.ToArray();
        mesh.SetTriangles(indices.ToArray(), 0);

        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
