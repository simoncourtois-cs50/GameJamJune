using Player.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Room.Runtime
{
    public class Manager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _player.OnMonsterKill += SwitchRoom;
            SwitchRoom();
        }

        #endregion


        #region Main API

        private void ResetAllRooms()
        {
           for(int i = 0; i < _roomDataList.Count; i++)
            {
                _roomDataList[i].m_isActive = false;
            }
        }

        private void SwitchRoom()
        {
            ResetAllRooms();

            do
            {
                _currentRoomIndex = Random.Range(0, _roomDataList.Count);
            }
            while (_currentRoomIndex == _previousRoomIndex);

            _roomDataList[_currentRoomIndex].m_isActive = true;
            Debug.Log(_roomDataList[_currentRoomIndex].m_name);
            _previousRoomIndex = _currentRoomIndex;
        }

        #endregion


        #region Private and Protected

        [SerializeField] private List<RoomData> _roomDataList;
        [SerializeField] private ManageTarget _player;
        private int _currentRoomIndex;
        private int _previousRoomIndex;
        #endregion
    }
}