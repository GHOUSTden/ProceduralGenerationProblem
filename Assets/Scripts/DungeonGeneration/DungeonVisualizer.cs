using UnityEngine;

public class DungeonVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonGeneration dungeonGeneration;
    [SerializeField] private GameObject roomCubePrefab;
    [SerializeField] private GameObject hallwayCubePrefab;
    [SerializeField] private GameObject uniqueRoomCubePrefab;
    
    public void VisualizeCell(DungeonCell cell)
    {
        Vector2Int cellCoordinates = cell.coordinates;
        Vector3 cellWorldPos = new Vector3(
            cellCoordinates.x * dungeonGeneration.cellSize,
            0f,
            cellCoordinates.y * dungeonGeneration.cellSize);
        
        switch (cell.cellType)
        {
            case CellType.Room:
                Instantiate(roomCubePrefab, cellWorldPos, Quaternion.identity);
                break;
            case CellType.Hallway:
                Instantiate(hallwayCubePrefab, cellWorldPos, Quaternion.identity);
                break;
            case CellType.Unique:
                Instantiate(uniqueRoomCubePrefab, cellWorldPos, Quaternion.identity);
                break;
        }
    }

    public void VisualizeRoom(DungeonRoom room)
    {
        Vector2Int roomCoordinates = room.bounds.position;
        Vector3 roomWorldPos = new Vector3(roomCoordinates.x, 0f, roomCoordinates.y);

        Instantiate(room.roomPrefab, roomWorldPos, Quaternion.identity);
    }
}
