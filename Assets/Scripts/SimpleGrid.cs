using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SimpleGrid : MonoBehaviour
{
    [SerializeField]
    SimpleCell cellPrefab;

    public List<SimpleCell> cells;

    [SerializeField]
    int width = 4, height = 3;

    SimpleMesh mesh;

    // Called when the script is loaded
    void Awake()
    {
        // Instantiate cell data
        cells = new List<SimpleCell>();

        // Create the cells
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                SimpleCell cell = Instantiate<SimpleCell>(cellPrefab);

                cell.position = new Vector3(
                    x * SimpleCell.apothem * 2f + z * SimpleCell.apothem - (z / 2) * SimpleCell.apothem * 2,
                    0,
                    z * SimpleCell.radius * 1.5f
                    );
                cell.transform.SetParent(this.transform, false);

                cell.x = x - z / 2;
                cell.z = z;

                cells.Add(cell);
            }
        }

        // Create a mesh from the cells
        mesh = GetComponentInChildren<SimpleMesh>();
        mesh.Triangulate(cells.ToArray());
    }

    // Called once per frame
    private void Update()
    {
        mesh.Triangulate(cells.ToArray());
    }

    // Return a cell from the grid based on its hex coordinates
    public SimpleCell GetCell(int x, int z)
    {
        try
        {
            return cells[x + z / 2 + z * width];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
}
