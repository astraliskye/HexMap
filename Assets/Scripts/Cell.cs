using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public static float radius = 1f;
    public static float apothem = radius * Mathf.Cos(Mathf.PI / 6);
    public static float heightUnit = 1f;

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

    public static Cell[] cells =
    {
        new Cell
        {
            name = "air",
            tangible = false,
            color = new Color(1, 1, 1)
        },
        new Cell
        {
            name = "dirt",
            tangible =  true,
            color = new Color(117f / 255, 65f / 255, 0)
        },
        new Cell
        {
            name = "water",
            tangible = true,
            color = new Color(3f / 255, 103f / 255, 161f / 255)
        },
        new Cell
        {
            name = "coast",
            tangible = true,
            color = new Color(242f / 255, 224f / 255, 131f / 255)
        }
    };

    public static Vector3 GetPositionByCoordinates(int x, int z)
    {
        float posX = x * Cell.apothem * 2 + (z % 2) * Cell.apothem;
        float posZ = z * Cell.radius * 1.5f;

        return new Vector3(posX, 0, posZ);
    }

    public string name;
    public bool tangible = true;
    public Color color;
}
