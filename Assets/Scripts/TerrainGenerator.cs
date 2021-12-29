using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkDirection
{
    NORTH,
    NORTHEAST,
    EAST,
    SOUTH,
    SOUTHWEST,
    WEST
};

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    Chunk chunkPrefab;

    [SerializeField]
    Transform playerTransform;

    FastNoise fastNoise = new FastNoise();
    (int x, int z) prevChunk;
    (int x, int z) currentChunk;
    int renderDistance = 5;

    static Dictionary<(int, int), int[,,]> worldCells;
    static Dictionary<(int, int), Chunk> chunks;
    List<(int x, int z)> chunksToCreate;
    List<Chunk> chunkPool;

    void OnEnable()
    {
        // Initialize variables
        worldCells = new Dictionary<(int, int), int[,,]>();
        chunks = new Dictionary<(int, int), Chunk>();
        chunksToCreate = new List<(int, int)>();
        chunkPool = new List<Chunk>();

        prevChunk = currentChunk = (
            (int)(playerTransform.position.x / (CellInfo.apothem * 2f * Chunk.chunkWidth)),
            (int)(playerTransform.position.z / (CellInfo.radius * 1.5f * Chunk.chunkLength)));

        // Create chunks immediately within the render distance
        for (int z = currentChunk.z - renderDistance; z <= currentChunk.z + renderDistance; z++)
        {
            for (int x = currentChunk.x - renderDistance; x < currentChunk.x + renderDistance; x++)
            {
                CreateChunk((x, z));
            }
        }

        for (int i = 0; i < worldCells[(0, 0)].GetLength(1); i++)
        {
            if (worldCells[(0, 0)][0, i, 0] == 0)
            {
                playerTransform.position = Vector3.up * (CellInfo.cellHeight * i + 2);
                break;
            }
        }
    }

    void Update()
    {
        (int x, int z) currentChunk = (
            Mathf.FloorToInt(playerTransform.position.x / (CellInfo.apothem * 2f * Chunk.chunkWidth)),
            Mathf.FloorToInt(playerTransform.position.z / (CellInfo.radius * 1.5f * Chunk.chunkLength)));

        // If the player has entered a new chunk
        // then the surrounding chunks may be recalculated
        if (prevChunk.x != currentChunk.x || prevChunk.z != currentChunk.z)
        {
            List<(int, int)> chunksToDestroy = new List<(int, int)>();

            // Queue chunks for destruction if they are outside the render distance
            foreach (KeyValuePair<(int x, int z), Chunk> c in chunks)
            {
                if (Mathf.Abs(currentChunk.x - c.Key.x) > renderDistance
                    || Mathf.Abs(currentChunk.z - c.Key.z) > renderDistance)
                {
                    chunksToDestroy.Add(c.Key);
                }
            }

            // Remove chunks in the creation queue that are outside the render distance
            for (int i = 0; i < chunksToCreate.Count; i++)
            {
                (int x, int z) coords = chunksToCreate[i];
                
                if (Mathf.Abs(currentChunk.x - coords.x) > renderDistance
                    || Mathf.Abs(currentChunk.z - coords.z) > renderDistance)
                {
                    chunksToCreate.RemoveAt(i);
                    i--;
                }
            }

            // Destroy the queued chunks and pool the inactive object
            foreach ((int, int) c in chunksToDestroy)
            {
                chunks[c].gameObject.SetActive(false);
                chunkPool.Add(chunks[c]);
                chunks.Remove(c);
            }

            // Queue chunks to be created that are within the render distance
            for (int z = currentChunk.z - renderDistance; z <= currentChunk.z + renderDistance; z++)
            {
                for (int x = currentChunk.x - renderDistance; x <= currentChunk.x + renderDistance; x++)
                {
                    if (!chunks.ContainsKey((x, z)) && !chunksToCreate.Contains((x, z)))
                        chunksToCreate.Add((x, z));
                }
            }

            prevChunk = currentChunk;

            StartCoroutine(DelayCreateChunks());
        }
    }

    IEnumerator DelayCreateChunks()
    {
        while (chunksToCreate.Count > 0)
        {
            Debug.Log("Creating chunk (" + chunksToCreate[0].x + ", " + chunksToCreate[0].z + ")");
            CreateChunk(chunksToCreate[0]);
            chunksToCreate.RemoveAt(0);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void CreateChunk((int x, int z) chunkPos)
    {
        int[,,] cells;

        if (worldCells.ContainsKey(chunkPos))
        {
            if (chunks.ContainsKey(chunkPos))
            {
                return;
            }

            cells = worldCells[chunkPos];
        }
        else
        {
            cells = new int[Chunk.chunkWidth, Chunk.chunkHeight, Chunk.chunkLength];

            for (int z = 0; z < Chunk.chunkLength; z++)
            {
                for (int x = 0; x < Chunk.chunkWidth; x++)
                {
                    Vector3 position = Chunk.chunkPositionFromCoordinates(chunkPos) +
                        CellInfo.cellPositionInChunk(x, 0, z);

                    int height = (int)(GenerateNoiseValue(position, 10) * Chunk.chunkHeight / 2 + Chunk.chunkHeight / 2);
                    for (int i = 0; i < Chunk.chunkHeight; i++)
                    {
                        if (i <= height)
                            cells[x, i, z] = 1;
                    }
                }
            }

            worldCells[chunkPos] = cells;
        }

        Chunk chunk;
        if (chunkPool.Count > 0)
        {
            chunk = chunkPool[0];
            chunkPool.RemoveAt(0);
            chunk.gameObject.SetActive(true);
        }
        else
        {
            chunk = Instantiate(chunkPrefab);
        }

        chunk.transform.position = Chunk.chunkPositionFromCoordinates(chunkPos);
        chunk.AddCells(cells);
        chunks.Add(chunkPos, chunk);

        TriangulateChunk(chunkPos);

        for (int i = 0; i < 6; i++)
        {
            (int, int) neighborPos = NeighborCoords(chunkPos, (ChunkDirection)i);

            if (chunks.ContainsKey(neighborPos))
                TriangulateChunk(neighborPos);
        }
    }

    void TriangulateChunk((int x, int y) coords)
    {
        int[,,] north, northEast, east, south, southWest, west;

        chunks[coords].Triangulate(
            worldCells.TryGetValue(NeighborCoords(coords, ChunkDirection.NORTH), out north) ? north : null,
            worldCells.TryGetValue(NeighborCoords(coords, ChunkDirection.NORTHEAST), out northEast) ? northEast : null,
            worldCells.TryGetValue(NeighborCoords(coords, ChunkDirection.EAST), out east) ? east : null,
            worldCells.TryGetValue(NeighborCoords(coords, ChunkDirection.SOUTH), out south) ? south : null,
            worldCells.TryGetValue(NeighborCoords(coords, ChunkDirection.SOUTHWEST), out southWest) ? southWest : null,
            worldCells.TryGetValue(NeighborCoords(coords, ChunkDirection.WEST), out west) ? west : null
            );
    }

    public static (int, int) NeighborCoords((int x, int z) coord, ChunkDirection direction)
    {
        return direction switch
        {
            ChunkDirection.NORTH => (coord.x, coord.z + 1),
            ChunkDirection.NORTHEAST => (coord.x + 1, coord.z + 1),
            ChunkDirection.EAST => (coord.x + 1, coord.z),
            ChunkDirection.SOUTH => (coord.x, coord.z - 1),
            ChunkDirection.SOUTHWEST => (coord.x - 1, coord.z - 1),
            ChunkDirection.WEST => (coord.x - 1, coord.z),
            _ => coord
        };
    }

    float GenerateNoiseValue(Vector3 position, int octaves)
    {
        /*float value = fastNoise.GetSimplex(position.x, position.z) + fastNoise.GetSimplex(position.x * 1.5f, position.z *  1.5f);
        value *= Mathf.Pow((fastNoise.GetSimplex(position.x * 0.01f, position.z * 0.01f) + 1) * 0.5f, 1.2f);
        return value / 2;*/

        float sum = 0;
        float cellSum = 0;
        float ampSum = 0;
        float amp = 1.0f;
        float freq = 0.2f;
        for (int i = 0; i < octaves; i++)
        {
            sum += amp * fastNoise.GetSimplex(position.x * freq, position.z * freq);
            cellSum += amp * fastNoise.GetCellular(position.x * freq, position.z * freq);
            ampSum += amp;

            amp -= 0.1f;
            freq += 0.1f;
        }

        return (sum + ampSum) / (ampSum * 2f);
    }
}




