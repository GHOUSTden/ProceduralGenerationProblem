using UnityEngine;
using System.Collections.Generic;

public class CycleConnectionSelector
{
    public List<RoomConnection> SelectExtraConnections(List<RoomConnection> candidateConnections, List<RoomConnection> mstConnections, float extraConnectionChance, System.Random random)
    {
        List<RoomConnection> extraConnections = new List<RoomConnection>();

        foreach (RoomConnection candidate in candidateConnections)
        {
            bool alreadyInMST = false;

            foreach (RoomConnection mstConnection in mstConnections)
            {
                if (candidate.HasSameEndpoints(mstConnection))
                {
                    alreadyInMST = true;
                    break;
                }
            }

            if (alreadyInMST)
            {
                continue;
            }

            if (random.NextDouble() < extraConnectionChance)
            {
                extraConnections.Add(candidate);
            }
        }

        return extraConnections;
    }
}
