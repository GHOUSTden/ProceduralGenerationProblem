using UnityEngine;

public class DelaunayEdge : MonoBehaviour
{
    public DelaunayVertex Start { get; }
    public DelaunayVertex End { get; }
    
    public bool isBad { get; set; }

    public DelaunayEdge(DelaunayVertex start, DelaunayVertex end)
    {
        Start = start;
        End = end;
    }

    public bool HasSameEndpoints(DelaunayEdge other)
    {
        bool sameDirection = Start.Id == other.Start.Id && End.Id == other.End.Id;

        bool oppositeDirection = Start.Id == other.End.Id && End.Id == other.Start.Id;

        return sameDirection || oppositeDirection;
    }
}
