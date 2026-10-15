using UnityEngine;

public class DungeonVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonGeneration dungeonGeneration;
    [SerializeField] private GameObject roomCubePrefab;
    [SerializeField] private GameObject hallwayCubePrefab;
    
    public void VisualizeCell(DungeonCell cell)
    {
        Vector2Int cellCoordinates = cell.coordinates;
        Vector3 cellWorldPos = new Vector3(
            cellCoordinates.x * dungeonGeneration.cellSize,
            0,
            cellCoordinates.y * dungeonGeneration.cellSize);

        switch (cell.cellType)
        {
            case CellType.Room:
                Instantiate(roomCubePrefab, cellWorldPos, Quaternion.identity);
                break;
            case CellType.Hallway:
                Instantiate(hallwayCubePrefab, cellWorldPos, Quaternion.identity);
                break;
        }
    }
}
