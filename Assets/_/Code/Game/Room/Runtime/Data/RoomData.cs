using UnityEngine;

namespace Room.Runtime
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
    public class RoomData : ScriptableObject
    {
        public RoomName m_name;
        public bool m_isActive;
    }
    public enum RoomName
    {
        Bedroom,
        Bathroom,
        Corridor,
        Kitchen,
        Living,
        Ceiling,
        Cave,
    }
}
