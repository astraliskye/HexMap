using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGrid : MonoBehaviour
{
    [SerializeField]
    SimpleCell cellPrefab;

    [SerializeField]
    int width = 4, height = 3;

    List<SimpleCell> cells;

    // Start is called before the first frame update
    void Awake()
    {
        cells = new List<SimpleCell>();

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                SimpleCell cell = Instantiate<SimpleCell>(cellPrefab);

                cell.transform.position = new Vector3(
                    x * SimpleCell.apothem * 2f + z * SimpleCell.apothem - (z / 2) * SimpleCell.apothem * 2,
                    0,
                    z * SimpleCell.radius * 1.5f
                    );
                cell.transform.SetParent(this.transform, false);

                cell.Triangulate();

                cells.Add(cell);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
