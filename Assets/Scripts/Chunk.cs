using UnityEngine;
using NoiseTest;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public const int SIZE = 16;
    public const int HEIGHT = 64;
    public const float WIDTH = Cell.APOTHEM * 2f * SIZE;
    public const float LENGTH = Cell.RADIUS * 1.5f * SIZE;

    public CellType[,,] cells = new CellType[SIZE, HEIGHT, SIZE];
    public (int, int) coordinates;

    static List<Vector3> vertices = new List<Vector3>();
    static List<int> triangles = new List<int>();
    static List<Vector2> topTerrainTypes = new List<Vector2>();

    Mesh mesh;
    MeshCollider meshCollider;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshCollider = GetComponent<MeshCollider>();
    }

    public void GenerateCells()
    {
        for (int z = 0; z < SIZE; z++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    cells[x, y, z] = GetCellType(x, y, z);
                }
            }
        }
    }

    public void TriangulateMesh()
    {
        vertices.Clear();
        triangles.Clear();
        topTerrainTypes.Clear();

        for (int z = 0; z < Chunk.SIZE; z++)
        {
            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int y = 0; y < Chunk.HEIGHT; y++)
                {
                    Vector3 topCenter = Cell.GetPositionFromCoordinates(x, z);
                    topCenter += new Vector3(0, y * Cell.HEIGHT, 0);

                    CellType cellType = cells[x, y, z];

                    if (cellType != CellType.AIR)
                    {
                        if (y < HEIGHT - 1 && cells[x, y + 1, z] == CellType.AIR)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                AddTriangle(
                                    topCenter,
                                    topCenter + Cell.corners[i],
                                    topCenter + Cell.corners[i + 1],
                                    Cell.cells[(int)cellType].topTerrain);
                            }
                        }

                        Vector3 bottomCenter = topCenter - new Vector3(0, Cell.HEIGHT, 0);

                        // Left side
                        if (x > 0)
                        {
                            // Left quad
                            if (cells[x - 1, y, z] == CellType.AIR)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[5],
                                    topCenter + Cell.corners[4],
                                    bottomCenter + Cell.corners[4],
                                    bottomCenter + Cell.corners[5],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }

                            // Top left quad
                            if (z < SIZE - 1)
                            {
                                if (z % 2 == 1 && cells[x, y, z + 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z % 2 == 0 && cells[x - 1, y, z + 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                            }
                            else
                            {
                                AddQuad(
                                    topCenter + Cell.corners[0],
                                    topCenter + Cell.corners[5],
                                    bottomCenter + Cell.corners[5],
                                    bottomCenter + Cell.corners[0],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }

                            // Bottom left quad
                            if (z > 0)
                            {
                                if (z % 2 == 0 && cells[x - 1, y, z - 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z % 2 == 1 && cells[x, y, z - 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                            }
                            else
                            {
                                AddQuad(
                                    topCenter + Cell.corners[4],
                                    topCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[4],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }
                        }
                        else
                        {
                            // Left quad
                            AddQuad(
                                topCenter + Cell.corners[5],
                                topCenter + Cell.corners[4],
                                bottomCenter + Cell.corners[4],
                                bottomCenter + Cell.corners[5],
                                Cell.cells[(int)cellType].sideTerrain
                                );

                            if (z % 2 == 0)
                            {
                                // Top left quad
                                AddQuad(
                                    topCenter + Cell.corners[0],
                                    topCenter + Cell.corners[5],
                                    bottomCenter + Cell.corners[5],
                                    bottomCenter + Cell.corners[0],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );

                                // Bottom left quad
                                AddQuad(
                                    topCenter + Cell.corners[4],
                                    topCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[3],
                                    bottomCenter + Cell.corners[4],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }
                            else
                            {
                                if (z < SIZE - 1 && cells[x, y, z + 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z == SIZE - 1)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[0],
                                        topCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[5],
                                        bottomCenter + Cell.corners[0],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }

                                if (z > 0 && cells[x, y, z - 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z == 0)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[4],
                                        topCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[3],
                                        bottomCenter + Cell.corners[4],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                            }
                        }

                        // Right side
                        if (x < SIZE - 1)
                        {
                            // Right quad
                            if (cells[x + 1, y, z] == CellType.AIR)
                            {
                                AddQuad(
                                    topCenter + Cell.corners[2],
                                    topCenter + Cell.corners[1],
                                    bottomCenter + Cell.corners[1],
                                    bottomCenter + Cell.corners[2],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }

                            // Top right quad
                            if (z < SIZE - 1)
                            {
                                if (z % 2 == 1 && cells[x + 1, y, z + 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[1],
                                        topCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[1],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z % 2 == 0 && cells[x, y, z + 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[1],
                                        topCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[1],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                            }
                            else
                            {
                                AddQuad(
                                    topCenter + Cell.corners[1],
                                    topCenter + Cell.corners[0],
                                    bottomCenter + Cell.corners[0],
                                    bottomCenter + Cell.corners[1],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }

                            // Bottom right quad
                            if (z > 0)
                            {
                                if (z % 2 == 0 && cells[x, y, z - 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z % 2 == 1 && cells[x + 1, y, z - 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                            }
                            else
                            {
                                AddQuad(
                                    topCenter + Cell.corners[3],
                                    topCenter + Cell.corners[2],
                                    bottomCenter + Cell.corners[2],
                                    bottomCenter + Cell.corners[3],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }
                        }
                        else
                        {
                            AddQuad(
                                topCenter + Cell.corners[2],
                                topCenter + Cell.corners[1],
                                bottomCenter + Cell.corners[1],
                                bottomCenter + Cell.corners[2],
                                Cell.cells[(int)cellType].sideTerrain
                                );

                            if (z % 2 == 1)
                            {
                                // Top right quad
                                AddQuad(
                                    topCenter + Cell.corners[1],
                                    topCenter + Cell.corners[0],
                                    bottomCenter + Cell.corners[0],
                                    bottomCenter + Cell.corners[1],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );

                                // Bottom right quad
                                AddQuad(
                                    topCenter + Cell.corners[3],
                                    topCenter + Cell.corners[2],
                                    bottomCenter + Cell.corners[2],
                                    bottomCenter + Cell.corners[3],
                                    Cell.cells[(int)cellType].sideTerrain
                                    );
                            }
                            else
                            {
                                if (z < SIZE - 1 && cells[x, y, z + 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[1],
                                        topCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[1],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z == SIZE - 1)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[1],
                                        topCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[0],
                                        bottomCenter + Cell.corners[1],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }

                                if (z > 0 && cells[x, y, z - 1] == CellType.AIR)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                                else if (z == 0)
                                {
                                    AddQuad(
                                        topCenter + Cell.corners[3],
                                        topCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[2],
                                        bottomCenter + Cell.corners[3],
                                        Cell.cells[(int)cellType].sideTerrain
                                        );
                                }
                            }
                        }
                    }
                }
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, topTerrainTypes);
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    CellType GetCellType(int x, int y, int z)
    {
        Vector3 cellPosition = Cell.GetPositionFromCoordinates(x + coordinates.Item1 * SIZE, z + coordinates.Item2 * SIZE);

        // Generate base height map
        float rawHeight = 0;

        float amplitude = 1f;
        float totalAmplitude = 0f;
        float frequency = 64f;

        for (int k = 0; k < 8; k++)
        {
            rawHeight += (float)(amplitude * (World.noiseGenerator.Evaluate(cellPosition.x / frequency, cellPosition.z / frequency, 0) + 1));

            totalAmplitude += amplitude;
            amplitude *= 0.5f;
            frequency /= 2;
        }

        rawHeight /= totalAmplitude * 2;

        // Modify height map
        float heightModifier = (float)(World.noiseGenerator.Evaluate(cellPosition.x / 97f, cellPosition.z / 97f, 0) + 1f) / 2f;
        if (heightModifier < 0) heightModifier = 0;

        float modifiedHeight = rawHeight * heightModifier;
        int height = (int)(modifiedHeight * HEIGHT);

        if (y < height)
        {
            return CellType.DIRT;
        }
        else if (y == height)
        {
            return CellType.GRASS;
        }
        else
        {
            return CellType.AIR;
        }
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 sideTerrain)
    {
        AddTriangle(a, b, c, sideTerrain);
        AddTriangle(c, d, a, sideTerrain);
    }

    void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 topTerrain)
    {
        int currentIndex = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(currentIndex);
        triangles.Add(currentIndex + 1);
        triangles.Add(currentIndex + 2);

        topTerrainTypes.Add(topTerrain);
        topTerrainTypes.Add(topTerrain);
        topTerrainTypes.Add(topTerrain);
    }
}
