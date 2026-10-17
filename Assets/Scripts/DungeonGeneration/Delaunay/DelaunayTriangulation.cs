using UnityEngine;
using System.Collections.Generic;

public class DelaunayTriangulation : MonoBehaviour
{
    private List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();

    private DelaunayTriangle superTriangle;
    public DelaunayTriangle SuperTriangle => superTriangle;

    public IReadOnlyList<DelaunayTriangle> Triangles => triangles;
    
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

    public void TriangulateVertices(List<DelaunayVertex> vertices)
    {
        Initialize(vertices);

        if (superTriangle == null)
        {
            return;
        }

        foreach (DelaunayVertex vertex in vertices)
        {
            List<DelaunayTriangle> badTriangles = FindBadTriangles(vertex);

            if (badTriangles.Count == 0)
            {
                Debug.LogWarning($"No affected triangles found for vertex {vertex.Id}.");
                
                continue;
            }

            List<DelaunayEdge> boundaryEdges = FindBoundaryEdges(badTriangles);

            foreach (DelaunayTriangle triangle in badTriangles)
            {
                triangles.Remove(triangle);
            }

            foreach (DelaunayEdge edge in boundaryEdges)
            {
                DelaunayTriangle newTriangle = new DelaunayTriangle(edge.Start, edge.End, vertex);
                
                triangles.Add(newTriangle);
            }
        }

        RemoveSuperTriangleTriangles();
        Debug.Log($"Delaunay triangulation finished: {triangles.Count} triangles.");
    }

    public List<RoomConnection> GetUniqueConnections()
    {
        List<RoomConnection> connections = new List<RoomConnection>();

        foreach (DelaunayTriangle triangle in triangles)
        {
            AddUniqueConnection(connections, triangle.A, triangle.B);
            AddUniqueConnection(connections, triangle.B, triangle.C);
            AddUniqueConnection(connections, triangle.C, triangle.A);
        }

        return connections;
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

    private List<DelaunayTriangle> FindBadTriangles(DelaunayVertex vertex)
    {
        List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();

        foreach (DelaunayTriangle triangle in triangles)
        {
            if (triangle.CircumcircleContains(vertex))
            {
                badTriangles.Add(triangle);
            }
        }

        return badTriangles;
    }

    private List<DelaunayEdge> FindBoundaryEdges(List<DelaunayTriangle> badTriangles)
    {
        List<DelaunayEdge> boundaryEdges = new List<DelaunayEdge>();

        foreach (DelaunayTriangle triangle in badTriangles)
        {
            AddOrCancelEdge(boundaryEdges, new DelaunayEdge(triangle.A, triangle.B));
            AddOrCancelEdge(boundaryEdges, new DelaunayEdge(triangle.B, triangle.C));
            AddOrCancelEdge(boundaryEdges, new DelaunayEdge(triangle.C, triangle.A));
        }

        return boundaryEdges;
    }

    private void AddOrCancelEdge(List<DelaunayEdge> edges, DelaunayEdge candidateEdge)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].HasSameEndpoints(candidateEdge))
            {
                edges.RemoveAt(i);
                return;
            }
        }
        
        edges.Add(candidateEdge);
    }

    private void RemoveSuperTriangleTriangles()
    {
        if (superTriangle == null)
        {
            return;
        }

        triangles.RemoveAll(triangle =>
            triangle.ContainsVertex(superTriangle.A) || 
            triangle.ContainsVertex(superTriangle.B) ||
            triangle.ContainsVertex(superTriangle.C));
    }

    private void AddUniqueConnection(List<RoomConnection> connections, DelaunayVertex start, DelaunayVertex end)
    {
        RoomConnection candidate = new RoomConnection(start, end);

        foreach (RoomConnection existing in connections)
        {
            if (existing.HasSameEndpoints(candidate))
            {
                return;
            }
        }
        
        connections.Add(candidate);
    }
}
