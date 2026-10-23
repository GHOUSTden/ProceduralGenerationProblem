using UnityEngine;
using System.Collections.Generic;

public class RoomConnectionPoint : MonoBehaviour
{
    public Vector2Int coordinates => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    public List<GameObject> connectedObjects;
    public bool isUsed = false;
}
