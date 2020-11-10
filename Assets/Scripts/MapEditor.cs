using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// User interface for editing the grid's mesh(es)
public class MapEditor : MonoBehaviour
{
    [SerializeField]
    GameGrid gameGrid;

    Slider slider;
    Cell activeCell;

    int randomSeed = 42;
    float randomness = 100f;

    // Called when the script is being loaded
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

                float x = hitPosition.x / (Cell.apothem * 2);
                float y = -x;

                float offset = hitPosition.z / (Cell.radius * 3f);
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
                activeCell = gameGrid.cells[new CellCoordinates(xCoord, zCoord).GridX + zCoord * gameGrid.width];
                slider.value = activeCell.position.y / Cell.elevationUnit;
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
            activeCell.position.y = elevation * Cell.elevationUnit;
            activeCell.chunk.Refresh();
        }
    }

    public void SetColor(int colorIndex)
    {
        if (activeCell != null)
        {
            activeCell.color = colorIndex;
            activeCell.chunk.Refresh();
        }
    }

    public void SetSeed(int seed)
    {
        randomSeed = seed;
    }

    public void SetRandomness(float randomness)
    {
        this.randomness = randomness;
    }    

    public void RandomizeCells()
    {
        List<Cell> cells = gameGrid.cells;

        Random.InitState(randomSeed);
        Vector3 offset = new Vector3(Random.Range(0f, randomness), Random.Range(0f, randomness), Random.Range(0f, randomness));

        foreach (Cell cell in cells)
        {
            int elevation = (int)(Mathf.Pow(Noise.noiseMethods[(int)NoiseMethodType.Perlin][2](cell.position, 0.05f) + 1f, 1f) * 4f);

            cell.position.y = elevation * Cell.elevationUnit;
            cell.color = elevation;
        }

        List<Chunk> chunks = gameGrid.chunks;

        foreach(Chunk chunk in chunks)
        {
            chunk.Refresh();
        }
    }
}