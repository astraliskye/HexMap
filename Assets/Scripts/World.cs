using System.Collections.Generic;
using UnityEngine;
using NoiseTest;
using System.Collections;

public class World : MonoBehaviour
{
    public Chunk chunkPrefab;
    public Transform player;

    public int distance = 5;

    public static OpenSimplexNoise noiseGenerator = new OpenSimplexNoise();
    public static Dictionary<(int x, int z), Chunk> chunks = new Dictionary<(int x, int z), Chunk>();

    (int x, int z) playerCurrentChunk = (-1, -1);

    private void Awake()
    {
        GenerateChunks();
    }

    private void Update()
    {
        GenerateChunks();
    }

    void GenerateChunks()
    {
        int playerX = (int)(player.position.x / Chunk.WIDTH);
        int playerZ = (int)(player.position.z / Chunk.LENGTH);

        if (playerCurrentChunk.x != playerX || playerCurrentChunk.z != playerZ)
        {
            playerCurrentChunk.x = playerX;
            playerCurrentChunk.z = playerZ;

            for (int z = -distance + playerZ; z < distance + playerZ; z++)
            {
                for (int x = -distance + playerX; x < distance + playerX; x++)
                {
                    if (!chunks.ContainsKey((x, z)))
                    {
                        BuildChunk(x, z);
                    }
                }
            }
        }
    }

    void BuildChunk(int x, int z)
    {
        Chunk chunk = Instantiate<Chunk>(chunkPrefab);
        chunks.Add((x, z), chunk);

        chunk.transform.position = new Vector3(x * Chunk.WIDTH, 0, z * Chunk.LENGTH);
        chunk.transform.SetParent(this.transform, false);
        chunk.coordinates = (x, z);
        chunk.GenerateCells();
        chunk.TriangulateMesh();
    }
}
