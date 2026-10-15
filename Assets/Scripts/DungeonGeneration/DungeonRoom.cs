using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom
{
    public Vector2Int roomSize;
    public Vector2Int roomCoordinates;
    public List<DungeonCell> cells;

    public void GenerateCells()
    {
        cells.Clear();
        
        for (int y = 0; y < roomSize.y; y++)
        {
            for (int x = 0; x < roomSize.x; x++)
            {
                Vector2Int takenCoordinates = new Vector2Int(
                    roomCoordinates.x + x, 
                    roomCoordinates.y + y);

                DungeonCell cell = new DungeonCell();

                cell.coordinates = takenCoordinates;
                cell.isOccupied = true;
                cell.cellType = CellType.Room;
                
                cells.Add(cell);
            }
        }
    }

    public bool CheckRoomIntersect(DungeonRoom room, DungeonRoom buffer)
    {
        return !((room.roomCoordinates.x >= (buffer.roomCoordinates.x + buffer.roomSize.x)) || ((room.roomCoordinates.x + room.roomSize.x) <= buffer.roomCoordinates.x)
            || (room.roomCoordinates.y >= (buffer.roomCoordinates.y + buffer.roomSize.y)) || ((room.roomCoordinates.y + room.roomSize.y) <= buffer.roomCoordinates.y));
    }
}
