using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

public class World : MonoBehaviour
{
    public Chunk chunkPrefab;
    public Transform playerTransform;

    [Range(1, 30)]
    public int waterLevel = 12;
    public int width = 4, height = 3;

    public static OpenSimplexNoise noiseGenerator;
    public static Dictionary<(int, int), Chunk> chunks;
    public static Dictionary<(int, int), bool> shouldTriangulate;

    private void Awake()
    {
        noiseGenerator = new OpenSimplexNoise();
        chunks = new Dictionary<(int, int), Chunk>();
        shouldTriangulate = new Dictionary<(int, int), bool>();
        Generate(0, 0);
    }

    private void Update()
    {
        Generate((int)(playerTransform.position.x / (Chunk.SIZE * Cell.apothem * 2)), (int)(playerTransform.position.z / (Chunk.SIZE * Cell.radius * 1.5f)));
    }

    void Generate(int playerX, int playerZ)
    {
        for (int z = -height * 2 + playerZ; z < height * 2 + playerZ; z++)
        {
            for (int x = -width * 2 + playerX; x < width * 2 + playerX; x++)
            {
                if (!chunks.ContainsKey((x, z)))
                {
                    Chunk chunk = Instantiate<Chunk>(chunkPrefab);
                    chunk.coordinates = (x, z);
                    chunk.GenerateCells(waterLevel);
                    chunk.transform.position = new Vector3(x * Cell.apothem * 2 * Chunk.SIZE, 0, z * Cell.radius * 1.5f * Chunk.SIZE);

                    chunk.transform.SetParent(this.transform, false);
                    chunks.Add((x, z), chunk);
                }
            }
        }

        for (int z = -height + playerZ; z < height + playerZ; z++)
        {
            for (int x = -width + playerX; x < width + playerX; x++)
            {
                if (!shouldTriangulate.ContainsKey((x, z)) || shouldTriangulate[(x, z)])
                {
                    chunks[(x, z)].TriangulateMesh();
                    shouldTriangulate[(x, z)] = false;
                }
            }
        }
    }
}
