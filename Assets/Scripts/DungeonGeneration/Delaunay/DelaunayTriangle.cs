using UnityEngine;

public class DelaunayTriangle
{
    public DelaunayVertex A { get; }
    public DelaunayVertex B { get; }
    public DelaunayVertex C { get; }
    
    public bool isBad { get; set; }

    public DelaunayTriangle(DelaunayVertex vertexA, DelaunayVertex vertexB, DelaunayVertex vertexC)
    {
        A = vertexA;
        B = vertexB;
        C = vertexC;
    }

    public bool ContainsVertex(DelaunayVertex vertex)
    {
        return A.Id == vertex.Id || B.Id == vertex.Id || C.Id == vertex.Id;
    }

    public bool CircumcircleContains(DelaunayVertex candidateVertex)
    {
        Vector2 pointA = A.Coordinate;
        Vector2 pointB = B.Coordinate;
        Vector2 pointC = C.Coordinate;

        float denomination = 2f * (pointA.x * (pointB.y - pointC.y) + pointB.x * (pointC.y - pointA.y) + pointC.x * (pointA.y - pointB.y));

        if (Mathf.Abs(denomination) < 0.00001f)
        {
            return false;
        }

        float pointASquared = pointA.sqrMagnitude;
        float pointBSquared = pointB.sqrMagnitude;
        float pointCSquared = pointC.sqrMagnitude;

        float circumcenterX =
            (pointASquared * (pointB.y - pointC.y) + 
             pointBSquared * (pointC.y - pointA.y) +
             pointCSquared * (pointA.y - pointB.y)) / denomination;
        
        float circumcenterY =
            (pointASquared * (pointC.y - pointB.y) + 
             pointBSquared * (pointA.y - pointC.y) +
             pointCSquared * (pointB.y - pointA.y)) / denomination;

        Vector2 circumcenter = new Vector2(circumcenterX, circumcenterY);

        float radiusSquared = (pointA - circumcenter).sqrMagnitude;

        float candidateDistanceSquared = (candidateVertex.Coordinate - circumcenter).sqrMagnitude;

        return candidateDistanceSquared <= radiusSquared;
    }
}
