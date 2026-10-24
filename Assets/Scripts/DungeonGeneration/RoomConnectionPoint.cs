using UnityEngine;
using System.Collections.Generic;

public class RoomConnectionPoint : MonoBehaviour
{
    public Vector2Int coordinates => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    public List<GameObject> connectedObjects;
    public bool isUsed = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, new Vector3(0.25f, 0.25f, 0.25f));
    }
}
