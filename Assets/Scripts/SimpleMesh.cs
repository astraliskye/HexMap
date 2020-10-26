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
    Dictionary<CellCoordinates, SimpleCell> cells;

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
    public void Triangulate(Dictionary<CellCoordinates, SimpleCell> cells)
    {
        this.cells = cells;

        // Trash old mesh data if mesh has previously been triangulated
        vertices.Clear();
        triangles.Clear();

        // Generate mesh data
        foreach (SimpleCell cell in cells.Values)
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
                center,
                center + SimpleCell.points[i],
                center + SimpleCell.points[i + 1]);
        }

        for (int i = 0; i < 3; i++)
        {
            CellCoordinates neighborCoordinates = CellCoordinates.GetNeighbor(cell.coordinates, i);
            SimpleCell neighbor;

            try
            {
                neighbor = cells[neighborCoordinates];
            }
            catch (KeyNotFoundException e)
            {
                continue;
            }

            Vector3 neighborCenter = neighbor.position;

            AddQuad(
                center + SimpleCell.points[i + 1],
                center + SimpleCell.points[i],
                neighborCenter + SimpleCell.points[i + 4],
                neighborCenter + SimpleCell.points[i + 3]
                );
        }
    }

    // Utility function to add a quad to the mesh data
    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        AddTriangle(a, b, c);
        AddTriangle(c, d, a);
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
