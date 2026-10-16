using System;
using UnityEngine;
using System.Collections.Generic;

public class GizmosVisualization : MonoBehaviour
{
    public List<Vector2> roomCenters;

    public void GetVertices(List<Vector2> vertices)
    {
        roomCenters = vertices;
    }
    
    public void OnDrawGizmos()
    {
        // GizmosVisualizeEdges();
        GizmosVisualizeVertices();
    }
    
    private void GizmosVisualizeEdges()
    {
        for (int i = 0; i < roomCenters.Count; i++)
        {
            for (int j = i + 1; j < roomCenters.Count; j++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(roomCenters[i], roomCenters[j]);
            }
        }
    }

    private void GizmosVisualizeVertices()
    {
        if (roomCenters == null)
        {
            return;
        }
        
        Gizmos.color = Color.green;

        foreach (var vertex in roomCenters)
        {
            Gizmos.DrawSphere(new Vector3(vertex.x, 0f, vertex.y), 0.5f);
        }
    }
}
