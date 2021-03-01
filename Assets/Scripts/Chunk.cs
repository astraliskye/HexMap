using UnityEngine;
using NoiseTest;
using System.Collections;

public class Chunk : MonoBehaviour
{
    public const int SIZE = 16;
    public const int MAX_HEIGHT = 128;

    public int[] cells;
    public (int, int) coordinates;

    HexMesh mesh;

    void Awake()
    {
        mesh = GetComponentInChildren<HexMesh>();
    }

    public void TriangulateMesh()
    {
        mesh.Triangulate(this);
    }

    public void GenerateCells(int waterLevel = 0)
    {
        cells = new int[SIZE * SIZE * MAX_HEIGHT];

        for (int z = 0; z < SIZE; z++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                Vector3 cellPosition = Cell.GetPositionByCoordinates(x + coordinates.Item1 * SIZE, z + coordinates.Item2 * SIZE);

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

                float heightModifier = (float)(World.noiseGenerator.Evaluate(cellPosition.x / 97f, cellPosition.z / 97f, 0) + 1f) / 2f;
                if (heightModifier < 0) heightModifier = 0;

                float modifiedHeight = rawHeight * heightModifier;
                int height = (int)(modifiedHeight * MAX_HEIGHT);

                int type = 1;

                if (height < waterLevel) type = 2;
                else if (height < waterLevel + 2) type = 3;


                GenerateColumn(x, z, height, type);
            }
        }
    }

    void GenerateColumn(int x, int z, int height, int type)
    {
        int columnIndex = 0;

        while (columnIndex < height - 1)
        {
            cells[columnIndex + x * MAX_HEIGHT + z * MAX_HEIGHT * SIZE] = 1;

            columnIndex++;
        }

        cells[columnIndex + x * MAX_HEIGHT + z * MAX_HEIGHT * SIZE] = type;

        columnIndex++;

        while (columnIndex < MAX_HEIGHT)
        {
            cells[columnIndex + x * MAX_HEIGHT + z * MAX_HEIGHT * SIZE] = 0;

            columnIndex++;
        }
    }
}
