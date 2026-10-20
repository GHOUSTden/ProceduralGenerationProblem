using System;
using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

[Serializable]
public class DungeonGeneration : MonoBehaviour
{
    [Header("Dungeon Settings")]
    [SerializeField] private int dungeonWidth;
    [SerializeField] private int dungeonDepth;
    public float cellSize;
    public float floorHeight;
    [SerializeField] private int roomPlacementAttempts;
    [SerializeField] private Vector2Int roomMaxSize;
    [SerializeField] private Vector2Int roomMinSize;
    [SerializeField] private List<RoomScriptableObject> uniqueRooms;
    
    [Header("Seed Generation")] 
    [SerializeField] private int currentSeed = 0;
    [SerializeField] private bool isCustomSeed = false;
    [SerializeField] private int customSeed = 10;
    
    [Header("Dungeon Connections")] 
    [SerializeField, Range(0f, 1f)] private float extraConnectionChance = 0.15f;
    private CycleConnectionSelector cycleConnectionSelector = new CycleConnectionSelector();
    private List<RoomConnection> extraConnections;
    private List<RoomConnection> finalConnections;
    
    [Header("References")] 
    [SerializeField] private DungeonVisualizer dungeonVisualizer;
    [SerializeField] private DelaunayTriangulation delaunayTriangulation;
    [SerializeField] private GizmosVisualization gizmosVisualization;

    private MinimumSpanningTree minimumSpanningTree = new MinimumSpanningTree();

    private AStarPathfinder aStarPathfinder = new AStarPathfinder();
    
    private List<RoomConnection> delaunayConnections;
    private List<RoomConnection> mstConnections;
    
    private List<DungeonRoom> rooms;
    private Random random;

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        if (isCustomSeed)
        {
            currentSeed = customSeed;
        }
        else
        {
            random = new Random();
            currentSeed = random.Next();
        }

        random = new Random(currentSeed);
        rooms = new List<DungeonRoom>();
        
        PlaceUniqueRooms();
        PlaceRegularRooms();
        Triangulate();
        GenerateHallways();
    }

    private void PlaceUniqueRooms()
    {
        foreach (RoomScriptableObject uniqueRoom in uniqueRooms)
        {
            Vector2Int location = new Vector2Int(0, 0);
            
            if (uniqueRoom.PlacementRule == PlacementRule.Fixed)
            {
                location = uniqueRoom.FixedCoordinates;
            }

            Vector2Int roomSize = uniqueRoom.RoomSize;

            DungeonRoom candidateRoom = new DungeonRoom();
            candidateRoom.bounds.position = location;
            candidateRoom.bounds.size = roomSize;
            CellType cellType = CellType.Unique;
            
            candidateRoom.GenerateCells(cellType);
            
            rooms.Add(candidateRoom);
            PlaceRoom(candidateRoom);
        }
    }
    
    private void PlaceRegularRooms()
    {
        for (int i = 0; i < roomPlacementAttempts; i++)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, dungeonWidth), 
                random.Next(0, dungeonDepth));

            Vector2Int roomSize = new Vector2Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMaxSize.y + 1));

            bool isValid = true;
            
            DungeonRoom candidateRoom = new DungeonRoom();
            candidateRoom.bounds.position = location;
            candidateRoom.bounds.size = roomSize;

            DungeonRoom bufferRoom = new DungeonRoom();
            bufferRoom.bounds.position = location + new Vector2Int(-1, -1);
            bufferRoom.bounds.size = roomSize + new Vector2Int(2, 2);

            foreach (var room in rooms)
            {
                if (room.CheckRoomIntersect(bufferRoom))
                {
                    isValid = false;
                    break;
                }
            }

            CellType cellType = CellType.Room;
            
            candidateRoom.GenerateCells(cellType);

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

    private void Triangulate()
    {
        List<DelaunayVertex> vertices = new List<DelaunayVertex>();

        for (int roomIndex = 0; roomIndex < rooms.Count; roomIndex++)
        {
            DungeonRoom room = rooms[roomIndex];

            Vector2 roomCenter = new Vector2((room.bounds.xMin + room.bounds.xMax - 1) / 2f, (room.bounds.yMin + room.bounds.yMax - 1) / 2f);

            DelaunayVertex vertex = new DelaunayVertex(roomIndex, roomCenter);
            
            vertices.Add(vertex);
        }
        Debug.Log($"All the {vertices.Count} vertices:");
        
        delaunayTriangulation.TriangulateVertices(vertices);

        delaunayConnections = delaunayTriangulation.GetUniqueConnections();

        mstConnections = minimumSpanningTree.Build(vertices, delaunayConnections);

        extraConnections = cycleConnectionSelector.SelectExtraConnections(delaunayConnections, mstConnections, extraConnectionChance, random);

        finalConnections = new List<RoomConnection>(mstConnections);
        finalConnections.AddRange(extraConnections);
        
        gizmosVisualization.GetVertices(vertices, cellSize);
        gizmosVisualization.GetTriangles(delaunayTriangulation.Triangles);
        gizmosVisualization.GetMSTConnections(mstConnections);
        gizmosVisualization.GetExtraConnections(extraConnections);
        
        Debug.Log($"Delaunay connections: {delaunayConnections.Count}");
        Debug.Log($"MST connections: {mstConnections.Count}");
        Debug.Log($"Extra connections: {extraConnections.Count}");
        Debug.Log($"Final connections: {finalConnections.Count}");
    }

    private void GenerateHallways()
    {
        if (finalConnections == null || finalConnections.Count == 0)
        {
            Debug.Log("No room connections to generate hallways for.");
            return;
        }

        HashSet<Vector2Int> roomCoordinates = GetRoomCoordinates();

        HashSet<Vector2Int> hallwayCoordinates = new HashSet<Vector2Int>();

        int failedPaths = 0;

        foreach (RoomConnection connection in finalConnections)
        {
            if (connection.Start.Id < 0 || connection.Start.Id >= rooms.Count || connection.End.Id < 0 || connection.End.Id >= rooms.Count)
            {
                Debug.LogWarning("A room connection contains an invalid room ID.");

                failedPaths++;
                continue;
            }

            DungeonRoom startRoom = rooms[connection.Start.Id];
            DungeonRoom endRoom = rooms[connection.End.Id];

            HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>(roomCoordinates);

            foreach (DungeonCell cell in startRoom.cells)
            {
                blockedCells.Remove(cell.coordinates);
            }

            foreach (DungeonCell cell in endRoom.cells)
            {
                blockedCells.Remove(cell.coordinates);
            }

            Vector2Int start = GetRoomGridCenter(startRoom);
            Vector2Int goal = GetRoomGridCenter(endRoom);

            List<Vector2Int> path = aStarPathfinder.FindPath(start, goal, blockedCells, dungeonWidth, dungeonDepth);

            if (path.Count == 0)
            {
                Debug.LogWarning($"Could not find a hallway between rooms {connection.Start.Id} and {connection.End.Id}.");

                failedPaths++;
                continue;
            }

            foreach (Vector2Int coordinate in path)
            {
                if (roomCoordinates.Contains(coordinate))
                {
                    continue;
                }

                if (!hallwayCoordinates.Add(coordinate))
                {
                    continue;
                }

                DungeonCell hallwayCell = new DungeonCell();

                hallwayCell.coordinates = coordinate;
                hallwayCell.cellType = CellType.Hallway;
                hallwayCell.isOccupied = true;
                
                dungeonVisualizer.VisualizeCell(hallwayCell);
            }
        }
        
        Debug.Log($"Hallway generation finished. " +
                  $"Created {hallwayCoordinates.Count} unique hallway cells. " +
                  $"Failed paths: {failedPaths}.");
    }
    
    private HashSet<Vector2Int> GetRoomCoordinates()
    {
        HashSet<Vector2Int> roomCoordinates = new HashSet<Vector2Int>();

        foreach (DungeonRoom room in rooms)
        {
            foreach (DungeonCell cell in room.cells)
            {
                roomCoordinates.Add(cell.coordinates);
            }
        }

        return roomCoordinates;
    }
    
    private Vector2Int GetRoomGridCenter(DungeonRoom room)
    {
        return new Vector2Int(Mathf.FloorToInt((room.bounds.xMin + room.bounds.xMax - 1) / 2f), Mathf.FloorToInt((room.bounds.yMin + room.bounds.yMax - 1) / 2f));
    }
    
    private void PlaceRoom(DungeonRoom room)
    {
        foreach (var cell in room.cells)
        {
            dungeonVisualizer.VisualizeCell(cell);
        }
    }
}
