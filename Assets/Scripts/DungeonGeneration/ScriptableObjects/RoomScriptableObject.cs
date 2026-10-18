using UnityEngine;

public enum RoomType
{
    Default
}

[CreateAssetMenu(fileName = "RoomScriptableObject", menuName = "ScriptableObject/Room")]
public class RoomScriptableObject : ScriptableObject
{
    [SerializeField] private RoomType roomType;
    public RoomType RoomType => roomType;
    
    [SerializeField] private Vector2Int roomSize;
    public Vector2Int RoomSize => roomSize;
    
    /*[SerializeField] private GameObject roomPrefab;
    public GameObject RoomPrefab => roomPrefab;*/
}
