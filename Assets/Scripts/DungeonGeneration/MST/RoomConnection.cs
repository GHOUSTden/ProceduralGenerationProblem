using UnityEngine;

public class RoomConnection
{
    public DelaunayVertex Start { get; }
    public DelaunayVertex End { get; }
    
    public float Weight { get; }

    public RoomConnection(DelaunayVertex start, DelaunayVertex end)
    {
        Start = start;
        End = end;

        Weight = Vector2.Distance(start.Coordinate, end.Coordinate);
    }

    public bool HasSameEndpoints(RoomConnection other)
    {
        bool sameDirection = Start.Id == other.Start.Id && End.Id == other.End.Id;
        
        bool oppositeDirection = Start.Id == other.End.Id && End.Id == other.Start.Id;
        
        return sameDirection || oppositeDirection;
    }
}
