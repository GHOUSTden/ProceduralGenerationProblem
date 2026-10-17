using UnityEngine;
using System.Collections.Generic;

public class MinimumSpanningTree
{
    public List<RoomConnection> Build(List<DelaunayVertex> vertices, List<RoomConnection> candidateConnections)
    {
        List<RoomConnection> mstConnections = new List<RoomConnection>();

        if (vertices == null || vertices.Count <= 1)
        {
            return mstConnections;
        }

        HashSet<int> connectedVertexIds = new HashSet<int>();

        connectedVertexIds.Add(vertices[0].Id);

        while (connectedVertexIds.Count < vertices.Count)
        {
            RoomConnection shortestConnection = null;

            foreach (RoomConnection connection in candidateConnections)
            {
                bool startIsConnected = connectedVertexIds.Contains(connection.Start.Id);

                bool endIsConnected = connectedVertexIds.Contains(connection.End.Id);

                if (startIsConnected == endIsConnected)
                {
                    continue;
                }

                if (shortestConnection == null || connection.Weight < shortestConnection.Weight)
                {
                    shortestConnection = connection;
                }
            }

            if (shortestConnection == null)
            {
                Debug.LogWarning("MST stopped: the candidate graph cannot connect every room.");
                
                break;
            }
            
            mstConnections.Add(shortestConnection);

            connectedVertexIds.Add(shortestConnection.Start.Id);
            connectedVertexIds.Add(shortestConnection.End.Id);
        }

        return mstConnections;
    }
}
