using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public const float RADIUS = 1f;
    public const float APOTHEM = RADIUS * 0.86602540f;
    public const float HEIGHT = 1f;

    public static Vector3[] corners =
    {
        new Vector3(0, 0, RADIUS),
        new Vector3(APOTHEM, 0, RADIUS / 2f),
        new Vector3(APOTHEM, 0, -RADIUS / 2f),
        new Vector3(0, 0, -RADIUS),
        new Vector3(-APOTHEM, 0, -RADIUS / 2f),
        new Vector3(-APOTHEM, 0, RADIUS / 2f),
        new Vector3(0, 0, RADIUS)
    };

    public static Cell[] cells =
    {
        new Cell
        {
            topTerrain = new Vector2(0f, 0f),
            sideTerrain = new Vector2(0f, 1f)
        },
        new Cell
        {
            topTerrain = new Vector2(2.0f, 0f),
            sideTerrain = new Vector2(2.0f, 1f)
        },
        new Cell
        {
            topTerrain = new Vector2(0f, 0f),
            sideTerrain = new Vector2(1.0f, 1f)
        },
        new Cell
        {
            topTerrain = new Vector2(0f, 0f),
            sideTerrain = new Vector2(0f, 1f)
        }
    };

    public static Vector3 GetPositionFromCoordinates(int x, int z)
    {
        float posX = x * APOTHEM * 2 + (z % 2) * APOTHEM;
        float posZ = z * RADIUS * 1.5f;

        return new Vector3(posX, 0, posZ);
    }

    public Vector2 topTerrain;
    public Vector2 sideTerrain;
}

public enum CellType { AIR = 0, DIRT = 1, GRASS = 2, WATER = 3 };