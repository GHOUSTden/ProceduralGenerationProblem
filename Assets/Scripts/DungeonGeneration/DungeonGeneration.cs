using System;
using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

[Serializable]
public class DungeonGeneration : MonoBehaviour
{
    [Header("Dungeon Settings")] 
    [SerializeField] private RectInt dungeonBounds;
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
            Vector2Int location = new Vector2Int(
                random.Next(0, dungeonBounds.position.x), 
                random.Next(0, dungeonBounds.position.y));
            
            if (uniqueRoom.PlacementRule == PlacementRule.Fixed)
            {
                location = uniqueRoom.FixedCoordinates;
            }

            if (uniqueRoom.PlacementRule == PlacementRule.Left || uniqueRoom.PlacementRule == PlacementRule.Right || uniqueRoom.PlacementRule == PlacementRule.Top || uniqueRoom.PlacementRule == PlacementRule.Down || uniqueRoom.PlacementRule == PlacementRule.Center)
            {
                location = GetDungeonBounds(uniqueRoom.PlacementRule, uniqueRoom.RoomSize);
            }
                
            Vector2Int roomSize = uniqueRoom.RoomSize;

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
            
            candidateRoom.GenerateCells(CellType.Unique);

            foreach (var cell in candidateRoom.cells)
            {
                if (cell.coordinates.x < 0 || cell.coordinates.x > dungeonBounds.position.x || cell.coordinates.y < 0 || cell.coordinates.y > dungeonBounds.position.y)
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
    
    private void PlaceRegularRooms()
    {
        for (int i = 0; i < roomPlacementAttempts; i++)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, dungeonBounds.position.x), 
                random.Next(0, dungeonBounds.position.y));

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
            
            candidateRoom.GenerateCells(CellType.Room);

            foreach (var cell in candidateRoom.cells)
            {
                if (cell.coordinates.x < 0 || cell.coordinates.x > dungeonBounds.position.x || cell.coordinates.y < 0 || cell.coordinates.y > dungeonBounds.position.y)
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

            List<Vector2Int> path = aStarPathfinder.FindPath(start, goal, blockedCells, dungeonBounds.position.x, dungeonBounds.position.y);

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

    private Vector2Int GetDungeonBounds(PlacementRule placementRule, Vector2Int roomSize)
    {
        int minX = 0;
        int maxX = dungeonBounds.xMax - roomSize.x;
                
        int minY = 0;
        int maxY = dungeonBounds.yMax - roomSize.y;
        
        switch (placementRule)
        {
            case PlacementRule.Left:
                maxX = dungeonBounds.xMax / 2 - roomSize.x;
                break;
            case PlacementRule.Right:
                minX = dungeonBounds.xMax / 2;
                break;
            case PlacementRule.Top:
                minY = dungeonBounds.yMax / 2;
                break;
            case PlacementRule.Down:
                maxY = dungeonBounds.yMax / 2 - roomSize.y;
                break;
            case PlacementRule.Center:
                minX = dungeonBounds.xMax / 4;
                maxX = dungeonBounds.xMax - minX - roomSize.x;

                minY = dungeonBounds.yMax / 4;
                maxY = dungeonBounds.yMax - minY - roomSize.y;
                break;
        }

        // Debug.Log($"Calculated Bounds For Placement Rule - {placementRule}: XMin - {minX}, XMax - {maxX}, YMin - {minY}, YMax - {maxY}.");
        
        gizmosVisualization.GetSpecifiedPlacementRule(minX, minY, maxX + roomSize.x, maxY + roomSize.y);
        
        Vector2Int finalCoordinates = new Vector2Int(
            random.Next(minX, maxX + 1), 
            random.Next(minY, maxY + 1));
        
        return finalCoordinates;
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
