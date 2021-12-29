using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public const int chunkWidth = 8;
    public const int chunkLength = 8;
    public const int chunkHeight = 128;

    int[,,] cells;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    Mesh mesh;
    MeshCollider meshCollider;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0, 0);
        Gizmos.DrawWireMesh(mesh, 0, transform.position, Quaternion.identity, Vector3.one);
    }

    public void AddCells(int[,,] cells)
    {
        this.cells = cells;
    }

    public static Vector3 chunkPositionFromCoordinates((int x, int z) chunkCoordinates)
    {
        return new Vector3(
            chunkCoordinates.x * CellInfo.apothem * 2 * chunkWidth,
            0,
            chunkCoordinates.z * CellInfo.radius * 1.5f * chunkLength
            );
    }

    public void Triangulate(int[,,] north, int[,,] northeast, int[,,] east, int[,,] south, int[,,] southwest, int[,,] west)
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();

        for (int z = 0; z < cells.GetLength(2); z++)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    // If the cell is not a zero, it should be rendered
                    if (cells[x, y, z] != 0)
                    {
                        Vector3 center = CellInfo.cellPositionInChunk(x, y, z);

                        // Render top
                        if (y == cells.GetLength(1) - 1 || cells[x, y + 1, z] == 0) {
                            for (int i = 0; i < 6; i++)
                            {
                                AddTriangle(
                                    center,
                                    center + CellInfo.corners[i],
                                    center + CellInfo.corners[(i + 1) % 6]
                                    );
                            }
                        }

                        // Left side
                        if (x > 0 && cells[x - 1, y, z] == 0
                            || x == 0 && west != null && west[west.GetLength(0) - 1, y, z] == 0)
                        {
                            AddQuad(
                                center + CellInfo.corners[5],
                                center + CellInfo.corners[4],
                                center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0),
                                center + CellInfo.corners[5] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }

                        // Right side
                        if (x < cells.GetLength(0) - 1 && cells[x + 1, y, z] == 0
                            || x == cells.GetLength(0) - 1 && east != null && east[0, y, z] == 0)
                        {
                            AddQuad(
                                center + CellInfo.corners[2],
                                center + CellInfo.corners[1],
                                center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0),
                                center + CellInfo.corners[2] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }

                        // Top left
                        if (x > 0 && z < cells.GetLength(2) - 1)    // inside chunk
                        {
                            if (z % 2 == 0 && cells[x - 1, y, z + 1] == 0)      // event rows
                            {
                                AddQuad(
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[5],
                                    center + CellInfo.corners[5] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && cells[x, y, z + 1] == 0)     // odd rows
                            {
                                AddQuad(
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[5],
                                    center + CellInfo.corners[5] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }
                        else if (z == cells.GetLength(2) - 1 && north != null && north[x, y, 0] == 0)   // top row
                        {
                            AddQuad(
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[5],
                                    center + CellInfo.corners[5] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }
                        else if (z < cells.GetLength(2) - 1 && x == 0)    // left side non-top row
                        {
                            if (z % 2 == 0 && west != null && west[west.GetLength(0) - 1, y, z + 1] == 0)   // even rows (outside chunk)
                            {
                                AddQuad(
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[5],
                                    center + CellInfo.corners[5] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && cells[x, y, z + 1] == 0)     // odd rows (inside chunk)
                            {
                                AddQuad(
                                        center + CellInfo.corners[0],
                                        center + CellInfo.corners[5],
                                        center + CellInfo.corners[5] + new Vector3(0, -CellInfo.cellHeight, 0),
                                        center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }

                        // Top right
                        if (x < cells.GetLength(0) - 1 && z < cells.GetLength(2) - 1)   // inside chunk
                        {
                            if (z % 2 == 0 && cells[x, y, z + 1] == 0)  // even rows
                            {
                                AddQuad(
                                    center + CellInfo.corners[1],
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && cells[x + 1, y, z + 1] == 0) // odd rows
                            {
                                AddQuad(
                                    center + CellInfo.corners[1],
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }
                        else if (x < cells.GetLength(0) - 1 & z == cells.GetLength(2) - 1 && north != null && north[x + 1, y, 0] == 0)  // top row
                        {
                            AddQuad(
                                    center + CellInfo.corners[1],
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }
                        else if (x == cells.GetLength(0) - 1 && z == cells.GetLength(2) - 1 && northeast != null && northeast[0, y, 0] == 0)    // top right
                        {
                            AddQuad(
                                    center + CellInfo.corners[1],
                                    center + CellInfo.corners[0],
                                    center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }
                        else if (x == cells.GetLength(0) - 1 && z < cells.GetLength(2) - 1)   // right edge
                        {
                            if (z % 2 == 0 && cells[x, y, z + 1] == 0)      // even rows (inside chunk)
                            {
                                AddQuad(
                                        center + CellInfo.corners[1],
                                        center + CellInfo.corners[0],
                                        center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0),
                                        center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && east != null && east[0, y, z + 1] == 0)      // odd rows (outside chunk)
                            {
                                AddQuad(
                                        center + CellInfo.corners[1],
                                        center + CellInfo.corners[0],
                                        center + CellInfo.corners[0] + new Vector3(0, -CellInfo.cellHeight, 0),
                                        center + CellInfo.corners[1] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }

                        // Bottom right
                        if (x < cells.GetLength(0) - 1 && z > 0)    // inside chunk
                        {
                            if (z % 2 == 0 && cells[x, y, z - 1] == 0)
                            {
                                AddQuad(
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[2],
                                    center + CellInfo.corners[2] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && cells[x + 1, y, z - 1] == 0)
                            {
                                AddQuad(
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[2],
                                    center + CellInfo.corners[2] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }
                        else if (z == 0 && south != null && south[x, y, south.GetLength(2) - 1] == 0) // bottom row
                        {
                            AddQuad(
                                center + CellInfo.corners[3],
                                center + CellInfo.corners[2],
                                center + CellInfo.corners[2] + new Vector3(0, -CellInfo.cellHeight, 0),
                                center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }
                        else if (x == cells.GetLength(0) - 1 && z > 0)      // right side not bottom row
                        {
                            if (z % 2 == 0 && cells[x, y, z - 1] == 0)  // even rows (inside chunk)
                            {
                                AddQuad(
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[2],
                                    center + CellInfo.corners[2] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && east != null && east[0, y, z - 1] == 0)  // odd rows (outside chunk)
                            {
                                AddQuad(
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[2],
                                    center + CellInfo.corners[2] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }

                        // Bottom left
                        if (x > 0 && z > 0)     // inside chunk
                        {
                            if (z % 2 == 0 && cells[x - 1, y, z - 1] == 0)
                            {
                                AddQuad(
                                    center + CellInfo.corners[4],
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && cells[x, y, z - 1] == 0)
                            {
                                AddQuad(
                                    center + CellInfo.corners[4],
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }
                        else if (x > 0 & z == 0 && south != null && south[x - 1, y, south.GetLength(2) - 1] == 0)  // bottom row
                        {
                            AddQuad(
                                    center + CellInfo.corners[4],
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }
                        else if (x == 0 && z == 0 && southwest != null && southwest[southwest.GetLength(0) - 1, y, southwest.GetLength(2) - 1] == 0)    // bottom left
                        {
                            AddQuad(
                                    center + CellInfo.corners[4],
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0));
                        }
                        else if (x == 0 && z > 0)   // left edge not bottom row
                        {
                            if (z % 2 == 0 && west != null && west[west.GetLength(0) - 1, y, z - 1] == 0)      // even rows (outside chunk)
                            {
                                AddQuad(
                                    center + CellInfo.corners[4],
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                            else if (z % 2 == 1 && cells[x, y, z - 1] == 0)      // odd rows (inside chunk)
                            {
                                AddQuad(
                                    center + CellInfo.corners[4],
                                    center + CellInfo.corners[3],
                                    center + CellInfo.corners[3] + new Vector3(0, -CellInfo.cellHeight, 0),
                                    center + CellInfo.corners[4] + new Vector3(0, -CellInfo.cellHeight, 0));
                            }
                        }
                    }
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        AddTriangle(a, b, c);
        AddTriangle(c, d, a);
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
