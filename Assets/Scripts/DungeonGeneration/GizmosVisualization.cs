using System;
using UnityEngine;
using System.Collections.Generic;

public class GizmosVisualization : MonoBehaviour
{
    private List<DelaunayVertex> roomCenters;

    private IReadOnlyList<DelaunayTriangle> triangles;

    private IReadOnlyList<RoomConnection> mstConnections;

    private IReadOnlyList<RoomConnection> extraConnections;

    private RectInt dungeonBounds;
    
    private float cellSize = 1f;

    public bool visualizeVertices = true;
    public bool visualizeTriangles = true;
    public bool visualizeMST = true;
    public bool visualizeExtraConnections = true;
    
    public PlacementRule placementRule;
    public bool visualizeSpecialPlacementRule = true;
    
    public void GetVertices(List<DelaunayVertex> vertices, float currentCellSize)
    {
        roomCenters = vertices;
        cellSize = currentCellSize;
    }

    public void GetTriangles(IReadOnlyList<DelaunayTriangle> currentTriangles)
    {
        triangles = currentTriangles;
    }

    public void GetMSTConnections(IReadOnlyList<RoomConnection> connections)
    {
        mstConnections = connections;
    }

    public void GetExtraConnections(IReadOnlyList<RoomConnection> connections)
    {
        extraConnections = connections;
    }

    public void GetDungeonBounds(RectInt db)
    {
        dungeonBounds = db;
    }
    
    private void OnDrawGizmos()
    {
        if (visualizeSpecialPlacementRule)
        {
            GizmosVisualizeSpecialPlacementRule();
        }
        
        if (visualizeTriangles)
        {
            GizmosVisualizeTriangles();
        }
        
        if (visualizeMST)
        {
            GizmosVisualizeMSTConnections();
        }

        if (visualizeExtraConnections)
        {
            GizmosVisualizeExtraConnections();
        }
        
        if (visualizeVertices)
        {
            GizmosVisualizeVertices();
        }
    }

    private Vector3 GetWorldPosition(DelaunayVertex vertex)
    {
        return new Vector3(vertex.Coordinate.x * cellSize, 0f, vertex.Coordinate.y * cellSize);
    }
    
    private void GizmosVisualizeVertices()
    {
        if (roomCenters == null)
        {
            return;
        }
        
        Gizmos.color = Color.green;

        foreach (DelaunayVertex vertex in roomCenters)
        {
            Gizmos.DrawSphere(GetWorldPosition(vertex), 0.5f * cellSize);
        }
    }

    private void GizmosVisualizeTriangles()
    {
        if (triangles == null)
        {
            return;
        }

        Gizmos.color = Color.green;

        foreach (DelaunayTriangle triangle in triangles)
        {
            Vector3 a = GetWorldPosition(triangle.A);
            Vector3 b = GetWorldPosition(triangle.B);
            Vector3 c = GetWorldPosition(triangle.C);
            
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, a);
        }
    }

    private void GizmosVisualizeMSTConnections()
    {
        if (mstConnections == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        foreach (RoomConnection connection in mstConnections)
        {
            Vector3 start = GetWorldPosition(connection.Start);
            Vector3 end = GetWorldPosition(connection.End);
            
            Gizmos.DrawLine(start, end);
        }
    }

    private void GizmosVisualizeExtraConnections()
    {
        if (extraConnections == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        foreach (RoomConnection connection in extraConnections)
        {
            Vector3 start = GetWorldPosition(connection.Start);
            Vector3 end = GetWorldPosition(connection.End);
            
            Gizmos.DrawLine(start, end);
        }
    }

    private void GizmosVisualizeSpecialPlacementRule()
    {
        if (dungeonBounds == null || placementRule == PlacementRule.Fixed || placementRule == PlacementRule.Random)
        {
            return;
        }

        int minX = 0;
        int maxX = dungeonBounds.xMax;
                
        int minY = 0;
        int maxY = dungeonBounds.yMax;
        
        Gizmos.color = Color.red;

        switch (placementRule)
        {
            case PlacementRule.Left:
                maxX = dungeonBounds.xMax / 2;
                break;
            case PlacementRule.Right:
                minX = dungeonBounds.xMax / 2;
                break;
            case PlacementRule.Top:
                minY = dungeonBounds.yMax / 2;
                break;
            case PlacementRule.Down:
                maxY = dungeonBounds.yMax / 2;
                break;
            case PlacementRule.Center:
                minX = dungeonBounds.xMax / 4;
                maxX = dungeonBounds.xMax - minX;

                minY = dungeonBounds.yMax / 4;
                maxY = dungeonBounds.yMax - minY;
                break;
        }
        
        Vector3 center = new Vector3(Mathf.FloorToInt((minX + maxX - 1) / 2f), 0f, Mathf.FloorToInt((minY + maxY - 1) / 2f));
        Vector3 size = new Vector3(maxX - minX + 1f, 1f, maxY - minY + 1f);
        
        Gizmos.DrawWireCube(center, size);
    }
}
