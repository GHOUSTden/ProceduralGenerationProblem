using System;
using UnityEngine;
using System.Collections.Generic;

public class GizmosVisualization : MonoBehaviour
{
    private List<DelaunayVertex> roomCenters;
    private DelaunayTriangle superTriangle;

    private float cellSize = 1f;
    
    public void GetVertices(List<DelaunayVertex> vertices, float currentCellSize)
    {
        roomCenters = vertices;
        cellSize = currentCellSize;
    }

    public void GetSuperTriangle(DelaunayTriangle triangle)
    {
        superTriangle = triangle;
    }
    
    private void OnDrawGizmos()
    {
        GizmosVisualizeVertices();
        GizmosVisualizeSuperTriangle();
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
            Gizmos.DrawSphere(new Vector3(vertex.Coordinate.x * cellSize, 0f, vertex.Coordinate.y * cellSize), 0.5f * cellSize);
        }
    }

    private void GizmosVisualizeSuperTriangle()
    {
        if (superTriangle == null)
        {
            return;
        }

        Vector3 a = new Vector3(superTriangle.A.Coordinate.x * cellSize, 0f, superTriangle.A.Coordinate.y * cellSize);
        Vector3 b = new Vector3(superTriangle.B.Coordinate.x * cellSize, 0f, superTriangle.B.Coordinate.y * cellSize);
        Vector3 c = new Vector3(superTriangle.C.Coordinate.x * cellSize, 0f, superTriangle.C.Coordinate.y * cellSize);

        Gizmos.color = Color.yellow;
        
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, a);
    }
}
