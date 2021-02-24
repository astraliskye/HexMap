using UnityEngine;
using NoiseTest;

public class Chunk : MonoBehaviour
{
    public static int size = 16;
    public Cell[] cells;
    public (int, int) coordinates;

    HexMesh mesh;

    OpenSimplexNoise noiseGenerator;

    void Awake()
    {
        mesh = GetComponentInChildren<HexMesh>();
        noiseGenerator = new OpenSimplexNoise(8);
    }

    void Start()
    {
        mesh.Triangulate(cells);
    }

    public void GenerateCells(int waterLevel = 0)
    {
        cells = new Cell[size * size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Cell cell = new Cell
                {
                    localPosition = new Vector3(j * Cell.apothem * 2 + (i % 2) * Cell.apothem, 0, i * Cell.radius * 1.5f),
                    type = 1
                };

                float worldX = cell.localPosition.x + transform.position.x;
                float worldZ = cell.localPosition.z + transform.position.z;

                float rawHeight = (float)(noiseGenerator.Evaluate(worldX / 16f, worldZ / 16f, 0) + 1f) * 32f;

                // float heightModifier = ((float)(noiseGenerator.Evaluate((cell.localPosition.x + transform.position.x) / 64f, (cell.localPosition.z + transform.position.z) / 64f, 0) + 1f)) / 2f;
                float heightModifier = -(1f/15000) * Mathf.Pow(worldX - 105, 2) - (1f / 15000) * Mathf.Pow(worldZ - 120, 2) + 1f;
                if (heightModifier < 0) heightModifier = 0;

                float modifiedHeight = rawHeight * heightModifier;
                int height = (int)modifiedHeight;

                if (height < waterLevel) cell.type = 2;

                cell.localPosition += new Vector3(0, height * Cell.heightUnit, 0);

                cells[j + i * size] = cell;
            }
        }

        mesh.Triangulate(cells);
    }
}
