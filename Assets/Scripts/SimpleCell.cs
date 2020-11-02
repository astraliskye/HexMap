using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to keep track of cell metrics and coordinates
public class SimpleCell : MonoBehaviour
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

    public static float elevationUnit = 1.5f;

    public Vector3 position;
    public int color;

    // Cell coordinates
    [SerializeField]
    public CellCoordinates coordinates;
}
