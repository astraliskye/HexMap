/*
 *  name:       HexMesh.cs
 *  purpose:    This class is responsible for triangulating a mesh from a set of
 *              given cells
 */
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexMesh : MonoBehaviour
{
    // Mesh data
    static List<Vector3> vertices;
    static List<int> triangles;
    static List<Color> colors;
    Mesh mesh;

    public Color[] colorPallet;

    MeshCollider meshCollider;

    // Called when script is being loaded
    void Awake()
    {
        // Initialize mesh data
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();

        meshCollider = GetComponent<MeshCollider>();
    }

    public void Triangulate(List<Cell> cells)
    {
        // Reset mesh data
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        // Generate mesh data
        foreach (Cell cell in cells)
        {
            Triangulate(cell);
        }

        // Attach data to mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        // Attach mesh to mesh collider
        meshCollider.sharedMesh = mesh;
    }


    // Triangulate a single cell
    void Triangulate(Cell cell)
    {
        // Triangulate cell proper
        Vector3 center = cell.position;

        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                center,
                center + Cell.points[i],
                center + Cell.points[i + 1],
                colorPallet[cell.color]);
        }

        // Triangulate cell connections
        for (int i = 0; i < 3; i++)
        {
            if (cell.neighbors[i] != null)
            {
                Vector3 neighborCenter = cell.neighbors[i].position;

                AddQuad(
                    center + Cell.points[i + 1],
                    center + Cell.points[i],
                    neighborCenter + Cell.points[i + 4],
                    neighborCenter + Cell.points[i + 3],
                    new Color(205f / 255, 133f / 255, 63f / 255)
                    );
            }
        }
    }

    // Utility function to add a quad to the mesh data
    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color)
    {
        AddTriangle(a, b, c, color);
        AddTriangle(c, d, a, color);
    }

    // Utility function to add a triangle to the mesh data
    // Suboptimal due to duplicate shared vertices
    void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        int currentIndex = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(currentIndex);
        triangles.Add(currentIndex + 1);
        triangles.Add(currentIndex + 2);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
}
