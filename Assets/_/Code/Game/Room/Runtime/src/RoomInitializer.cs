using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Room.Runtime
{
    public class RoomInitializer : MonoBehaviour
    {
        #region Unity API

        private void Start()
        {
            if (GetActivityStatus()) SpawnMonster();
        }

        #endregion


        #region Main API

        private bool GetActivityStatus()
        {
            return _roomData.m_isActive;
        }

        private void SpawnMonster()
        {
            int randomIndex = Random.Range(0, _monsterPrefabList.Count);
            Instantiate(_monsterPrefabList[randomIndex], _spawnPositionList[randomIndex].position, Quaternion.identity);
        }

        #endregion


        #region Private and protected

        [SerializeField] private RoomData _roomData;
        [SerializeField] private List<Transform> _spawnPositionList = new();
        [SerializeField] private List<GameObject> _monsterPrefabList = new();

        #endregion
    }
}
