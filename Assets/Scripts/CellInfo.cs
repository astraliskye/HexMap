using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CellInfo
{
    public const float radius = 5;
    public const float apothem = radius * 0.86602540378f;

    public static Vector3[] corners = {
        new Vector3(0, 0, radius),
        new Vector3(apothem, 0, radius / 2),
        new Vector3(apothem, 0, -radius / 2),
        new Vector3(0, 0, -radius),
        new Vector3(-apothem, 0, -radius / 2),
        new Vector3(-apothem, 0, radius / 2),
    };

    public const float cellHeight = 1f;

    public static Vector3 cellPositionInChunk(int x, int y, int z)
    {
        return new Vector3(
            x * apothem * 2 + (z % 2) * apothem,
            y * cellHeight,
            z * radius * 1.5f);

    }
}
