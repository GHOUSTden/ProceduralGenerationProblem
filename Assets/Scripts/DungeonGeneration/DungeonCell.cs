using UnityEngine;

public enum CellType
{
    Room,
    Hallway,
}

public class DungeonCell
{
    public Vector2Int coordinates;
    public CellType cellType;
    public bool isOccupied;
}
