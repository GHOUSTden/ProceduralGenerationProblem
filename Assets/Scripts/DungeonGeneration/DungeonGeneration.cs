using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

public class DungeonGeneration : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private int dungeonWidth;
    [SerializeField] private int dungeonDepth;
    public float cellSize;
    public float floorHeight;
    [SerializeField] private int roomCount;
    [SerializeField] private Vector2Int roomMaxSize;
    [SerializeField] private Vector2Int roomMinSize;

    [Header("References")] 
    [SerializeField] private DungeonVisualizer dungeonVisualizer;
    [SerializeField] private GameObject roomCubePrefab;
    [SerializeField] private GameObject hallwayCubePrefab;
    
    private List<DungeonRoom> rooms;
    private Random random;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        random = new Random(0);
        rooms = new List<DungeonRoom>();
        
        PlaceRooms();
    }

    private void PlaceRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, dungeonWidth), 
                random.Next(0, dungeonDepth));

            Vector2Int roomSize = new Vector2Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMaxSize.y));

            bool isValid = true;
            
            DungeonRoom candidateRoom = new DungeonRoom();
            candidateRoom.roomCoordinates = location;
            candidateRoom.roomSize = roomSize;

            DungeonRoom bufferRoom = new DungeonRoom();
            bufferRoom.roomCoordinates = location + new Vector2Int(-1, -1);
            bufferRoom.roomSize = roomSize + new Vector2Int(2, 2);

            foreach (var room in rooms)
            {
                if (CheckRoomIntersect(room, bufferRoom))
                {
                    isValid = false;
                    break;
                }
            }
            
            candidateRoom.GenerateCells();

            foreach (var cell in candidateRoom.cells)
            {
                if (cell.coordinates.x < 0 || cell.coordinates.x > dungeonWidth || cell.coordinates.y < 0 || cell.coordinates.y > dungeonDepth)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                rooms.Add(candidateRoom);
                PlaceRoom(candidateRoom);
            }
        }
    }
    
    private bool CheckRoomIntersect(DungeonRoom room, DungeonRoom buffer)
    {
        return !((room.roomCoordinates.x >= (buffer.roomCoordinates.x + buffer.roomSize.x)) || ((room.roomCoordinates.x + room.roomSize.x) <= buffer.roomCoordinates.x)
            || (room.roomCoordinates.y >= (buffer.roomCoordinates.y + buffer.roomSize.y)) || ((room.roomCoordinates.y + room.roomSize.y) <= buffer.roomCoordinates.y));
    }

    private void PlaceRoom(DungeonRoom room)
    {
        Vector2Int roomPosition = room.roomCoordinates;
        Vector2Int roomSize = room.roomSize;
    }
}
