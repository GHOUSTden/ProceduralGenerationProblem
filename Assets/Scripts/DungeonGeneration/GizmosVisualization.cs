using System;
using UnityEngine;
using System.Collections.Generic;

public class GizmosVisualization : MonoBehaviour
{
    private List<DelaunayVertex> roomCenters;

    private IReadOnlyList<DelaunayTriangle> triangles;

    private float cellSize = 1f;
    
    public void GetVertices(List<DelaunayVertex> vertices, float currentCellSize)
    {
        roomCenters = vertices;
        cellSize = currentCellSize;
    }

    public void GetTriangles(IReadOnlyList<DelaunayTriangle> currentTriangles)
    {
        triangles = currentTriangles;
    }
    
    private void OnDrawGizmos()
    {
        GizmosVisualizeVertices();
        GizmosVisualizeTriangles();
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
}
