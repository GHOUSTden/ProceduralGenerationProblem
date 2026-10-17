using UnityEngine;
using System.Collections.Generic;
using Random = System.Random;

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
    [SerializeField] private DelaunayTriangulation delaunayTriangulation;
    [SerializeField] private GizmosVisualization gizmosVisualization;

    private MinimumSpanningTree minimumSpanningTree = new MinimumSpanningTree();

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
        random = new Random(10);
        rooms = new List<DungeonRoom>();
        
        PlaceRooms();
        Triangulate();
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
        
        gizmosVisualization.GetVertices(vertices, cellSize);
        gizmosVisualization.GetTriangles(delaunayTriangulation.Triangles);
        gizmosVisualization.GetMSTConnections(mstConnections);
        
        Debug.Log($"Delaunay connections: {delaunayConnections.Count}");

        Debug.Log($"MST connections: {mstConnections.Count}");
    }
    
    private void PlaceRoom(DungeonRoom room)
    {
        foreach (var cell in room.cells)
        {
            dungeonVisualizer.VisualizeCell(cell);
        }
    }
}
