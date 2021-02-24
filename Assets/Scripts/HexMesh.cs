using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    static List<Vector3> vertices;
    static List<int> triangles;
    static List<Color> colors;
    Mesh mesh;

    void Awake()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
    }

    public void Triangulate(Cell[] cells)
    {
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
    }


    void Triangulate(Cell cell)
    {
        Vector3 topCenter = cell.localPosition;

        Color cellColor;

        switch (cell.type)
        {
            case 0:
                cellColor = new Color(1, 1, 1);
                break;
            case 1:
                cellColor = new Color(205f / 255, 133f / 255, 63f / 255);
                break;
            case 2:
                cellColor = new Color(0, 0, 1);
                break;
            default:
                cellColor = new Color(0, 1, 0);
                break;
        }

        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                topCenter,
                topCenter + Cell.corners[i],
                topCenter + Cell.corners[i + 1],
                cellColor);
        }

        Vector3 bottomCenter = new Vector3(cell.localPosition.x, 0f, cell.localPosition.z);

        for (int i = 0; i < 6; i++)
        {
            AddQuad(
                topCenter + Cell.corners[i + 1],
                topCenter + Cell.corners[i],
                bottomCenter + Cell.corners[i],
                bottomCenter + Cell.corners[i + 1],
                cellColor);
        }

        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                bottomCenter,
                bottomCenter + Cell.corners[i + 1],
                bottomCenter + Cell.corners[i],
                cellColor);
        }
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color)
    {
        AddTriangle(a, b, c, color);
        AddTriangle(c, d, a, color);
    }

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
