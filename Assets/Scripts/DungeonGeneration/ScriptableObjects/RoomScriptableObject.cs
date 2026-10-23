using UnityEngine;
using System.Collections.Generic;

public enum RoomType
{
    Regular,
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

[CreateAssetMenu(fileName = "RoomScriptableObject", menuName = "ScriptableObject/Room")]
public class RoomScriptableObject : ScriptableObject
{
    
    [SerializeField] private RoomType roomType;
    public RoomType RoomType => roomType;

    [SerializeField] private PlacementRule placementRule;
    public PlacementRule PlacementRule => placementRule;

    [SerializeField] private Vector2Int fixedCoordinates;
    public Vector2Int FixedCoordinates => fixedCoordinates;
    
    [SerializeField] private Vector2Int roomSize;
    public Vector2Int RoomSize => roomSize;

    [SerializeField] private List<RoomConnectionPoint> roomConnectionPoints;
    public List<RoomConnectionPoint> RoomConnectionPoints => roomConnectionPoints;

    [SerializeField] private GameObject roomPrefab;
    public GameObject RoomPrefab => roomPrefab;
}
