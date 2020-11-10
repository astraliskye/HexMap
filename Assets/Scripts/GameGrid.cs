/*
 *  name:       GameGrid.cs
 *  purpose:    Class to represent the overall grid, containing
 *              references to every cell
 */
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    // Prefabs that the grid will need to instantiate
    [SerializeField]
    Cell cellPrefab;
    [SerializeField]
    Chunk chunkPrefab;

    public int width = 4, height = 3;

    // Collections of every cell and chunk that appears in the grid
    public List<Cell> cells;
    public List<Chunk> chunks;

    // Called when the script is loaded
    void Awake()
    {
        cells = new List<Cell>();

        // Instantiate chunks
        for (int z = 0; z <= height / Chunk.size; z++)
        {
            for (int x = 0; x <= width / Chunk.size; x++)
            {
                Chunk chunk = Instantiate<Chunk>(chunkPrefab);
                chunk.coordinates = (x, z);
                chunk.transform.SetParent(this.transform, false);
                chunks.Add(chunk);
            }
        }

        // Instantiate cells
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                InstantiateCell(x, z);
            }
        }
    }

    void InstantiateCell(int x, int z)
    {
        Cell cell = Instantiate<Cell>(cellPrefab);

        cell.position = new Vector3(
            x * Cell.apothem * 2f + z * Cell.apothem - (z / 2) * Cell.apothem * 2,
            0,
            z * Cell.radius * 1.5f
            );
        cell.coordinates = new CellCoordinates(x - z / 2, z);
        cell.color = 0;     // Default cell color

        // Set neighbors
        int index = x + z * width;
        if (x % width != 0)
        {
            cell.neighbors[4] = cells[index - 1];
            cells[index - 1].neighbors[1] = cell;
        }

        if (z > 0)
        {
            if (z % 2 == 1)
            {
                if (x != width - 1)
                {
                    cell.neighbors[2] = cells[index - width + 1];
                    cells[index - width + 1].neighbors[5] = cell;
                }

                cell.neighbors[3] = cells[index - width];
                cells[index - width].neighbors[0] = cell;
            }
            else
            {
                if (x != 0)
                {
                    cell.neighbors[3] = cells[index - width - 1];
                    cells[index - width - 1].neighbors[0] = cell;
                }

                cell.neighbors[2] = cells[index - width];
                cells[index - width].neighbors[5] = cell;
            }
        }

        cells.Add(cell);

        // Add cell to appropriate chunk
        int chunkX = x / Chunk.size;
        int chunkZ = z / Chunk.size;

        Chunk chunk = chunks[chunkX + chunkZ * (width / Chunk.size + 1)];

        chunk.cells.Add(cell);
        cell.chunk = chunk;
        cell.transform.SetParent(chunk.transform, false);
    }
}
