using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public static float radius = 1f;
    public static float apothem = radius * Mathf.Cos(Mathf.PI / 6);
    public static float heightUnit = .5f;

    public static Vector3[] corners =
    {
        new Vector3(0, 0, radius),
        new Vector3(apothem, 0, radius / 2f),
        new Vector3(apothem, 0, -radius / 2f),
        new Vector3(0, 0, -radius),
        new Vector3(-apothem, 0, -radius / 2f),
        new Vector3(-apothem, 0, radius / 2f),
        new Vector3(0, 0, radius)
    };

    public int type = 1;
    public Vector3 localPosition = new Vector3(0, 0, 0);
}
