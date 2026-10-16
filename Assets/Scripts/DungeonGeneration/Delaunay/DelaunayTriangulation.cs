using System;
using UnityEngine;
using System.Collections.Generic;

public class DelaunayTriangulation : MonoBehaviour
{
    private List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();

    private DelaunayTriangle superTriangle;
    public DelaunayTriangle SuperTriangle => superTriangle;
    
    public void Initialize(List<DelaunayVertex> vertices)
    {
        triangles.Clear();
        superTriangle = null;

        if (vertices == null || vertices.Count == 0)
        {
            return;
        }

        superTriangle = CreateSuperTriangle(vertices);
        
        triangles.Add(superTriangle);
    }

    private DelaunayTriangle CreateSuperTriangle(List<DelaunayVertex> vertices)
    {
        float minX = vertices[0].Coordinate.x;
        float maxX = vertices[0].Coordinate.x;

        float minY = vertices[0].Coordinate.y;
        float maxY = vertices[0].Coordinate.y;

        int maxVertexId = vertices[0].Id;

        foreach (DelaunayVertex vertex in vertices)
        {
            Vector2 coordinate = vertex.Coordinate;

            minX = Mathf.Min(minX, coordinate.x);
            maxX = Mathf.Max(maxX, coordinate.x);

            minY = Mathf.Min(minY, coordinate.y);
            maxY = Mathf.Min(maxY, coordinate.y);

            maxVertexId = Mathf.Max(maxVertexId, vertex.Id);
        }

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        float size = Mathf.Max(maxX - minX, maxY - minY);

        if (size < 0.0001f)
        {
            size = 1f;
        }

        Vector2 leftCoordinate = new Vector2(centerX - size * 20f, centerY - size);
        Vector2 topCoordinate = new Vector2(centerX, centerY + size * 20f);
        Vector2 rightCoordinate = new Vector2(centerX + size * 20f, centerY - size);

        DelaunayVertex leftVertex = new DelaunayVertex(maxVertexId + 1, leftCoordinate);
        DelaunayVertex topVertex = new DelaunayVertex(maxVertexId + 2, topCoordinate);
        DelaunayVertex rightVertex = new DelaunayVertex(maxVertexId + 3, rightCoordinate);

        return new DelaunayTriangle(leftVertex, topVertex, rightVertex);
    }
}
