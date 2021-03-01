using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexMesh : MonoBehaviour
{
    static List<Vector3> vertices;
    static List<int> triangles;
    static List<Color> colors;
    Mesh mesh;
    MeshCollider meshCollider;

    void Awake()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Triangulate(Chunk chunk)
    {
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int z = 0; z < Chunk.SIZE; z++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int y = 0; y < Chunk.MAX_HEIGHT; y++)
                {
                    Vector3 topCenter = Cell.GetPositionByCoordinates(x, z);
                    topCenter += new Vector3(0, y * Cell.heightUnit, 0);

                    int cellIndex = y + x * Chunk.MAX_HEIGHT + z * Chunk.MAX_HEIGHT * Chunk.SIZE;

                    int cellType = chunk.cells[cellIndex];

                    if (Cell.cells[cellType].tangible)
                    {
                        if (y == Chunk.MAX_HEIGHT - 1 || !Cell.cells[chunk.cells[cellIndex + 1]].tangible)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                AddTriangle(
                                    topCenter,
                                    topCenter + Cell.corners[i],
                                    topCenter + Cell.corners[i + 1],
                                    Cell.cells[cellType].color);
                            }
                        }

                        Vector3 bottomCenter = topCenter - new Vector3(0, Cell.heightUnit, 0);

                        // On right boundary
                        if (x == Chunk.SIZE - 1)
                        {
                            // Top right side within chunk
                            if (z % 2 == 0 && !Cell.cells[chunk.cells[cellIndex + Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[1],
                                    topCenter + Cell.corners[0],
                                    bottomCenter + Cell.corners[0],
                                    bottomCenter + Cell.corners[1],
                                    Cell.cells[cellType].color);
                            }

                            // Bottom right corner
                            if (z == 0)
                            {
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1, chunk.coordinates.Item2 - 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1, chunk.coordinates.Item2 - 1)].cells[y + Chunk.MAX_HEIGHT * (Chunk.SIZE - 1) + Chunk.MAX_HEIGHT * (Chunk.SIZE - 1) * Chunk.SIZE]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[cellType].color);
                                }
                            }
                            // Bottom right side within chunk
                            else if (z % 2 == 0 && !Cell.cells[chunk.cells[cellIndex - Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[3],
                                    topCenter + Cell.corners[2],
                                    bottomCenter + Cell.corners[2],
                                    bottomCenter + Cell.corners[3],
                                    Cell.cells[cellType].color);
                            }

                            if (World.chunks.ContainsKey((chunk.coordinates.Item1 + 1, chunk.coordinates.Item2))) {
                                // Right side
                                if (!Cell.cells[World.chunks[(chunk.coordinates.Item1 + 1, chunk.coordinates.Item2)].cells[y + z * Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[2],
                                        topCenter + Cell.corners[1],
                                        bottomCenter + Cell.corners[1],
                                        bottomCenter + Cell.corners[2],
                                        Cell.cells[cellType].color);
                                }

                                // Top right side
                                if (z < Chunk.SIZE - 1)
                                {
                                    if (z % 2 == 1) {
                                        if (!Cell.cells[World.chunks[(chunk.coordinates.Item1 + 1, chunk.coordinates.Item2)].cells[y + (z + 1) * Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                                        {
                                            AddQuad(
                                                topCenter + Cell.corners[1],
                                                topCenter + Cell.corners[0],
                                                bottomCenter + Cell.corners[0],
                                                bottomCenter + Cell.corners[1],
                                                Cell.cells[cellType].color);
                                        }
                                    }
                                }
                                // Top right corner
                                else if (z == Chunk.SIZE - 1)
                                {
                                    if (World.chunks.ContainsKey((chunk.coordinates.Item1 + 1, chunk.coordinates.Item2 + 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1 + 1, chunk.coordinates.Item2 + 1)].cells[y]].tangible)
                                    {
                                        AddQuad(
                                            topCenter + Cell.corners[1],
                                            topCenter + Cell.corners[0],
                                            bottomCenter + Cell.corners[0],
                                            bottomCenter + Cell.corners[1],
                                            Cell.cells[cellType].color);
                                    }
                                }

                                // Bottom right side
                                if (z > 0)
                                {
                                    if (z % 2 == 1 && !Cell.cells[World.chunks[(chunk.coordinates.Item1 + 1, chunk.coordinates.Item2)].cells[y + (z - 1) * Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                                    {
                                        AddQuad(
                                            topCenter + Cell.corners[3],
                                            topCenter + Cell.corners[2],
                                            bottomCenter + Cell.corners[2],
                                            bottomCenter + Cell.corners[3],
                                            Cell.cells[cellType].color);
                                    }
                                }
                            }
                        }
                        // Within right boundary
                        else if (x < Chunk.SIZE - 1)
                        {
                            // Right side
                            if ((!Cell.cells[chunk.cells[cellIndex + Chunk.MAX_HEIGHT]].tangible)) {
                                AddQuad(
                                    topCenter + Cell.corners[2],
                                    topCenter + Cell.corners[1],
                                    bottomCenter + Cell.corners[1],
                                    bottomCenter + Cell.corners[2],
                                    Cell.cells[cellType].color);
                            }

                            // Top right side
                            // Within top boundary
                            if (z < Chunk.SIZE - 1)
                            {
                                if (z % 2 == 0 ? !Cell.cells[chunk.cells[cellIndex + Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible : !Cell.cells[chunk.cells[cellIndex + (Chunk.SIZE + 1) * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[1],
                                        topCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[1],
                                        Cell.cells[cellType].color);
                                }
                            }
                            // On top boundary
                            else if (z == Chunk.SIZE - 1)
                            {
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1, chunk.coordinates.Item2 + 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1, chunk.coordinates.Item2 + 1)].cells[y + (x + 1) * Chunk.MAX_HEIGHT]].tangible) {
                                    AddQuad(
                                        topCenter + Cell.corners[1],
                                        topCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[1],
                                        Cell.cells[cellType].color);
                                }
                            }

                            // Bottom right side
                            // Within bottom boundary
                            if (z > 0)
                            {
                                if (z % 2 == 0 ? !Cell.cells[chunk.cells[cellIndex - Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible : !Cell.cells[chunk.cells[cellIndex - (Chunk.SIZE - 1) * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[cellType].color);
                                }
                            }
                            // On bottom boundary
                            else if (z == 0) {
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1, chunk.coordinates.Item2 - 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1, chunk.coordinates.Item2 - 1)].cells[y + x * Chunk.MAX_HEIGHT + (Chunk.SIZE - 1) * Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[cellType].color);
                                }
                            }
                        }

                        // On left boundary
                        if (x == 0)
                        {
                            // Top left side within chunk
                            if (z < Chunk.SIZE - 1 && z % 2 == 1 && !Cell.cells[chunk.cells[cellIndex + Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[0],
                                    topCenter + Cell.corners[5],
                                    bottomCenter + Cell.corners[5],
                                    bottomCenter + Cell.corners[0],
                                    Cell.cells[cellType].color);
                            }

                            // Bottom left side outside of chunk
                            if (z > 0 && z % 2 == 0 && World.chunks.ContainsKey((chunk.coordinates.Item1 - 1, chunk.coordinates.Item2)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1 - 1, chunk.coordinates.Item2)].cells[y + (Chunk.SIZE - 1) * Chunk.MAX_HEIGHT + (z - 1) * Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[4],
                                    topCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[4],
                                    Cell.cells[cellType].color);
                            }

                            // Bottom left side within chunk
                            if (z % 2 == 1 && !Cell.cells[chunk.cells[cellIndex - Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[4],
                                    topCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[4],
                                    Cell.cells[cellType].color);
                            }

                            // Left side
                            if (World.chunks.ContainsKey((chunk.coordinates.Item1 - 1, chunk.coordinates.Item2)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1 - 1, chunk.coordinates.Item2)].cells[y + (Chunk.SIZE - 1) * Chunk.MAX_HEIGHT + z * Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[5],
                                    topCenter + Cell.corners[4],
                                    bottomCenter + Cell.corners[4],
                                    bottomCenter + Cell.corners[5],
                                    Cell.cells[cellType].color);
                            }

                            if (World.chunks.ContainsKey((chunk.coordinates.Item1 - 1, chunk.coordinates.Item2)))
                            {
                                // Top left side outside of chunk
                                if (z < Chunk.SIZE - 1)
                                {
                                    if (z % 2 == 0 && !Cell.cells[World.chunks[(chunk.coordinates.Item1 - 1, chunk.coordinates.Item2)].cells[y + (Chunk.SIZE - 1) * Chunk.MAX_HEIGHT + (z + 1) * Chunk.MAX_HEIGHT * Chunk.SIZE]].tangible)
                                    {
                                        AddQuad(
                                            topCenter + Cell.corners[0],
                                            topCenter + Cell.corners[5],
                                            bottomCenter + Cell.corners[5],
                                            bottomCenter + Cell.corners[0],
                                            Cell.cells[cellType].color);
                                    }
                                }
                            }

                            // On top boundary
                            if (z == Chunk.SIZE - 1)
                            {
                                // Top left corner
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1, chunk.coordinates.Item2 + 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1, chunk.coordinates.Item2 + 1)].cells[y]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[cellType].color);
                                }
                            }
                            // On bottom boundary
                            else if (z == 0)
                            {
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1 - 1, chunk.coordinates.Item2 - 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1 - 1, chunk.coordinates.Item2 - 1)].cells[y + (Chunk.SIZE - 1) * Chunk.MAX_HEIGHT + (Chunk.SIZE - 1) * Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[cellType].color);
                                }
                            }
                        }
                        // Within left boundary
                        else if (x > 0)
                        {
                            // Left side
                            if (!Cell.cells[chunk.cells[cellIndex - Chunk.MAX_HEIGHT]].tangible)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[5],
                                    topCenter + Cell.corners[4],
                                    bottomCenter + Cell.corners[4],
                                    bottomCenter + Cell.corners[5],
                                    Cell.cells[cellType].color);
                            }

                            // Top left side
                            // Within top boundary
                            if (z < Chunk.SIZE - 1)
                            {
                                // Top left
                                if (z % 2 == 0 ? !Cell.cells[chunk.cells[cellIndex + (Chunk.SIZE - 1) * Chunk.MAX_HEIGHT]].tangible : !Cell.cells[chunk.cells[cellIndex + Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[cellType].color);
                                }
                            }
                            // On top boundary
                            else if (z == Chunk.SIZE - 1)
                            {
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1, chunk.coordinates.Item2 + 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1, chunk.coordinates.Item2 + 1)].cells[y + x * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[cellType].color);
                                }
                            }

                            // Bottom left side
                            // Within bottom boundary
                            if (z > 0)
                            {
                                if (z % 2 == 0 ? !Cell.cells[chunk.cells[cellIndex - (Chunk.SIZE + 1) * Chunk.MAX_HEIGHT]].tangible : !Cell.cells[chunk.cells[cellIndex - Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[cellType].color);
                                }
                            }
                            // On bottom boundary
                            else if (z == 0)
                            {
                                if (World.chunks.ContainsKey((chunk.coordinates.Item1, chunk.coordinates.Item2 - 1)) && !Cell.cells[World.chunks[(chunk.coordinates.Item1, chunk.coordinates.Item2 - 1)].cells[y + (x - 1) * Chunk.MAX_HEIGHT + (Chunk.SIZE - 1) * Chunk.SIZE * Chunk.MAX_HEIGHT]].tangible)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[cellType].color);
                                }
                            }
                        }

                        if (y != 0 && !Cell.cells[chunk.cells[cellIndex - 1]].tangible)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                AddTriangle(
                                    bottomCenter,
                                    bottomCenter + Cell.corners[i + 1],
                                    bottomCenter + Cell.corners[i],
                                    Cell.cells[cellType].color);
                            }
                        }
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
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
