using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom
{
    public RectInt bounds;
    public List<DungeonCell> cells = new List<DungeonCell>();

    public void GenerateCells(CellType cellType)
    {
        cells.Clear();
        
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                Vector2Int takenCoordinates = new Vector2Int(
                    bounds.position.x + x, 
                    bounds.position.y + y);

                DungeonCell cell = new DungeonCell();

                cell.coordinates = takenCoordinates;
                cell.isOccupied = true;
                cell.cellType = cellType;
                
                cells.Add(cell);
            }
        }
    }

    public bool CheckRoomIntersect(DungeonRoom buffer)
    {
        return !((bounds.position.x >= (buffer.bounds.position.x + buffer.bounds.size.x)) || ((bounds.position.x + bounds.size.x) <= buffer.bounds.position.x)
            || (bounds.position.y >= (buffer.bounds.position.y + buffer.bounds.size.y)) || ((bounds.position.y + bounds.size.y) <= buffer.bounds.position.y));
    }
}
