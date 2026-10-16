using System;
using UnityEngine;
using System.Collections.Generic;

public class GizmosVisualization : MonoBehaviour
{
    public List<DelaunayVertex> roomCenters;

    private float cellSize = 1f;
    
    public void GetVertices(List<DelaunayVertex> vertices, float currentCellSize)
    {
        roomCenters = vertices;
        cellSize = currentCellSize;
    }
    
    private void OnDrawGizmos()
    {
        GizmosVisualizeVertices();
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
}
