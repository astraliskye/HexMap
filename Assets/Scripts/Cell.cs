/*
 *  name:       Cell.cs
 *  purpose:    Contain attributes that are the same for every cell
 *              and other data that will make creating and modifying
 *              the grid easier
 */
using UnityEngine;

public class Cell : MonoBehaviour
{
    // Static variables that apply to every cell
    public static float radius = 3f;
    public static float apothem = 0.86602540378f * radius;

    public static Vector3[] points =
    {
        new Vector3(0, 0, radius),
        new Vector3(apothem, 0, radius * 0.5f),
        new Vector3(apothem, 0, -radius * 0.5f),
        new Vector3(0, 0, -radius),
        new Vector3(-apothem, 0, -radius * 0.5f),
        new Vector3(-apothem, 0, radius * 0.5f),
        new Vector3(0, 0, radius)
    };

    public static float elevationUnit = apothem;
    public static float numElevations = 64;

    public Vector3 position;
    public int color;

    public Cell[] neighbors = { null, null, null, null, null, null };

    // Cell coordinates
    [SerializeField]
    public CellCoordinates coordinates;

    public Chunk chunk;
}
