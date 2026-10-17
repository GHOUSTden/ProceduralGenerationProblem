using UnityEngine;
using System.Collections.Generic;
using Vector2 = System.Numerics.Vector2;

public class AStarPathfinder
{
    private static readonly Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, HashSet<Vector2Int> blockedCells, int gridWidth, int gridDepth)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();

        if (!IsInsideGrid(start, gridWidth, gridDepth) || !IsInsideGrid(goal, gridWidth, gridDepth))
        {
            return new List<Vector2Int>();
        }

        if (blockedCells.Contains(start) || blockedCells.Contains(goal))
        {
            return new List<Vector2Int>();
        }
        
        openSet.Add(start);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            Vector2Int current = GetLowestFCostNode(openSet, gScore, goal);

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;

                if (!IsInsideGrid(neighbor, gridWidth, gridDepth))
                {
                    continue;
                }

                if (blockedCells.Contains(neighbor) || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int tentativeGScore = gScore[current] + 1;

                if (gScore.TryGetValue(neighbor, out int existingGScore) && tentativeGScore >= existingGScore)
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector2Int>();
    }

    private Vector2Int GetLowestFCostNode(List<Vector2Int> openSet, Dictionary<Vector2Int, int> gScore, Vector2Int goal)
    {
        Vector2Int bestNode = openSet[0];

        int lowestFCost = int.MaxValue;
        int lowestHCost = int.MaxValue;

        foreach (Vector2Int node in openSet)
        {
            int currentGCost = gScore[node];
            int currentHCost = GetHeuristic(node, goal);

            int currentFCost = currentGCost + currentHCost;

            if (currentFCost < lowestFCost || (currentFCost == lowestFCost && currentHCost < lowestHCost))
            {
                bestNode = node;
                lowestFCost = currentFCost;
                lowestHCost = currentHCost;
            }
        }

        return bestNode;
    }

    private int GetHeuristic(Vector2Int current, Vector2Int goal)
    {
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    private bool IsInsideGrid(Vector2Int position, int gridWidth, int gridDepth)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridDepth;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        
        path.Add(current);

        while (cameFrom.TryGetValue(current, out Vector2Int previous))
        {
            current = previous;
            path.Add(current);
        }
        
        path.Reverse();

        return path;
    }
}
