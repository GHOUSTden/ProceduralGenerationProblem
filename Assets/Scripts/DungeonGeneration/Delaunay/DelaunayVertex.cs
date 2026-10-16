using UnityEngine;

public class DelaunayVertex
{
    public int Id { get; }
    public Vector2 Coordinate { get; }

    public DelaunayVertex(int id, Vector2 coordinate)
    {
        Id = id;
        Coordinate = coordinate;
    }
}
