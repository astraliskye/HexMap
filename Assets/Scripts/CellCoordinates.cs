/*
 *  name:       CellCoordinates.cs
 *  purpose:    Class to keep track of a cell's coordintates and convert between
 *              the grid's offset x coordinates and the cell's non-offset x coordinate
 */

public struct CellCoordinates
{
    public int x, z;

    // Get the x coordinate in terms of the offset coordinate scheme
    public int GridX
    {
        get
        {
            return x + z / 2;
        }
    }

    public CellCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    // Get coordintes of the neighboring cell in a certian direction
    // 0 - up-right, 1 - right, 2 - down-right, 3 - down-left, 4 - left, 5 - up-left
    public static CellCoordinates GetNeighbor(CellCoordinates coordinates, int direction)
    {

        CellCoordinates neighbor = coordinates;

        switch (direction)
        {
            case 0:
                neighbor.z += 1;
                break;
            case 1:
                neighbor.x += 1;
                break;
            case 2:
                neighbor.z -= 1;
                neighbor.x += 1;
                break;
            case 3:
                neighbor.z -= 1;
                break;
            case 4:
                neighbor.x -= 1;
                break;
            case 5:
                neighbor.z += 1;
                neighbor.x -= 1;
                break;
            default:
                break;
        }

        return neighbor;
    }
}