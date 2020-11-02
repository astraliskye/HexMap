using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// User interface for editing the grid's mesh(es)
public class SimpleMapEditor : MonoBehaviour
{
    [SerializeField]
    SimpleGrid simpleGrid;

    Slider slider;
    SimpleCell activeCell;

    // Called when the script is loaded
    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        slider.value = 0;
        activeCell = null;
    }

    // Called once per frame
    void Update()
    {
        // Check for clicks on the mesh
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Determine the position of the click and translate it to hexagonal coordinates
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(inputRay, out hit))
            {
                Vector3 hitPosition = hit.point; // transform.InverseTransformPoint(hit.point);

                float x = hitPosition.x / (SimpleCell.apothem * 2);
                float y = -x;

                float offset = hitPosition.z / (SimpleCell.radius * 3f);
                x -= offset;
                y -= offset;

                int xCoord = Mathf.RoundToInt(x);
                int yCoord = Mathf.RoundToInt(y);
                int zCoord = Mathf.RoundToInt(-x -y);

                // Check for rounding errors and correct them
                // The sum of the elements of the coordinate should be zero
                if (xCoord + yCoord + zCoord != 0)
                {
                    float dX = Mathf.Abs(x - xCoord);
                    float dY = Mathf.Abs(y - yCoord);
                    float dZ = Mathf.Abs(-x - y - zCoord);

                    // If the x coordinate has the greatest rounding error, correct it
                    if (dX > dY && dX > dZ)
                    {
                        xCoord = -yCoord - zCoord;
                    }
                    // Otherwise correct z
                    else if (dZ > dY)
                    {
                        zCoord = -xCoord - yCoord;
                    }
                }
                activeCell = simpleGrid.GetCell(new CellCoordinates(xCoord, zCoord));
                slider.value = activeCell.position.y / SimpleCell.elevationUnit;
                Toggle[] toggles = GetComponentsInChildren<Toggle>();
                toggles[activeCell.color].isOn = true;
            }
        }
    }

    // Set the elevation of the selected cell
    public void SetElevation(float elevation)
    {
        if (activeCell != null)
        {
            activeCell.position.y = elevation * SimpleCell.elevationUnit;
        }
    }

    public void SetColor(int colorIndex)
    {
        if (activeCell != null)
        {
            activeCell.color = colorIndex;
        }
    }
}