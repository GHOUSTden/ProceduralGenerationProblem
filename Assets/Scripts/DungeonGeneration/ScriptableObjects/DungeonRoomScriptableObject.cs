using UnityEngine;

public enum RoomType
{
    Entrance,
    Boss
}

public enum PlacementRule
{
    Fixed,      // Place at fixed coordinates, defined by the developer
    Random,     // Random placement
    Left,       
    Right,
    Top,        // Specific zone bounds
    Down,
    Center
}

[CreateAssetMenu(fileName = "DungeonRoomScriptableObject", menuName = "ScriptableObject/DungeonRoomScriptableObject")]
public class DungeonRoomScriptableObject : ScriptableObject
{
    [SerializeField] private RoomType roomType;
    public RoomType RoomType => roomType;

    [SerializeField] private PlacementRule placementRule;
    public PlacementRule PlacementRule => placementRule;

    [SerializeField] private RectInt roomBounds;
    public RectInt RoomBounds => roomBounds;
    
    [SerializeField] private GameObject roomPrefab;
    public GameObject RoomPrefab => roomPrefab;
}
