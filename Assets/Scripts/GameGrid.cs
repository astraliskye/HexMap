using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField]
    Chunk chunkPrefab;

    [Range(1, 30)]
    public int waterLevel = 12;
    public int width = 4, height = 3;

    public List<Chunk> chunks;

    private void Awake()
    {
        GenerateGrid(waterLevel: waterLevel);
    }

    void GenerateGrid(int waterLevel = 0)
    {
        chunks.Clear();

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Chunk chunk = Instantiate<Chunk>(chunkPrefab);
                chunk.coordinates = (x, z);
                chunk.transform.position = new Vector3(x * Cell.apothem * 2 * Chunk.size, 0, z * Cell.radius * 1.5f * Chunk.size);

                chunk.GenerateCells(waterLevel);

                chunk.transform.SetParent(this.transform, false);
                chunks.Add(chunk);
            }
        }
    }
}
