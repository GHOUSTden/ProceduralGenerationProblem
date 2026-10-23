using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LocationScriptableObject", menuName = "ScriptableObject/Location")]
public class LocationScriptableObject : ScriptableObject
{
    [SerializeField] private List<RoomScriptableObject> uniqueRooms;
    public List<RoomScriptableObject> UniqueRooms => uniqueRooms;

    [SerializeField] private List<RoomScriptableObject> regularRooms;
    public List<RoomScriptableObject> RegularRooms => regularRooms;
}
