using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleCell : MonoBehaviour
{
    public static float radius = 1f;
    public static float apothem = 0.86602540378f * radius;

    public static Vector3[] points =
    {
        new Vector3(0, 0, radius),
        new Vector3(apothem, 0, radius * 0.5f),
        new Vector3(apothem, 0, -radius * 0.5f),
        new Vector3(0, 0, -radius),
        new Vector3(-apothem, 0, -radius * 0.5f),
        new Vector3(-apothem, 0, radius * 0.5f),
        new Vector3(0, 0, radius)
    };

    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    public void Triangulate()
    {
        vertices.Clear();
        triangles.Clear();

        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                Vector3.zero,
                points[i],
                points[i + 1]);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        int currentIndex = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(currentIndex);
        triangles.Add(currentIndex + 1);
        triangles.Add(currentIndex + 2);
    }
}
