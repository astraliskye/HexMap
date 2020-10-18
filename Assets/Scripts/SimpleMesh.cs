using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class for triangulating a mesh of hexagons
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class SimpleMesh : MonoBehaviour
{
    // Mesh data
    List<Vector3> vertices;
    List<int> triangles;
    Mesh mesh;

    MeshCollider meshCollider;

    // Called when script is loaded
    void Awake()
    {
        // Initialize mesh data
        vertices = new List<Vector3>();
        triangles = new List<int>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();

        // Get mesh collider
        meshCollider = GetComponent<MeshCollider>();
    }

    // Triangulate a list of cells
    public void Triangulate(SimpleCell[] cells)
    {
        // Trash old mesh data if mesh has previously been triangulated
        vertices.Clear();
        triangles.Clear();

        // Generate mesh data
        foreach (SimpleCell cell in cells)
        {
            Triangulate(cell);
        }

        // Attach data to mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Attach mesh to mesh collider
        meshCollider.sharedMesh = mesh;
    }


    // Triangulate a single cell
    void Triangulate(SimpleCell cell)
    {
        Vector3 center = cell.position;

        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                cell.position,
                cell.position + SimpleCell.points[i],
                cell.position + SimpleCell.points[i + 1]);
        }
    }


    // Utility function to add a triangle to the mesh data
    // Suboptimal due to duplicate shared vertices
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
