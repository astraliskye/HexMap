/*
 *  name:       Chunk.cs
 *  purpose:    Chunks contain a subset of cells that it will pass
 *              on to a hex mesh so the grid can be divided from
 *              one big mesh into smaller meshes
 */
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    // Chunk data
    public static int size = 40;
    public List<Cell> cells = new List<Cell>();
    public (int, int) coordinates;

    HexMesh mesh;

    // Called when script is being loaded
    void Awake()
    {
        mesh = GetComponentInChildren<HexMesh>();
    }

    // Called on the frame when the script is enabled
    void Start()
    {
        mesh.Triangulate(cells);
    }

    public void Refresh()
    {
        mesh.Triangulate(cells);
    }
}
