using Player.Runtime;
using UnityEngine;

namespace GameConductor.Runtime
{
    public class GameManager : MonoBehaviour
    {

        #region Main API

        private void DecrementKillCount()
        {

        }

        #endregion


        #region Private and Protected

        [Header("References")]
        [SerializeField] private GameObject _gameoverScreen;
        [SerializeField] private GameObject _victoryScreen;
        [SerializeField] private ManageTarget _player;

        [Header("Game Variables")]
        [SerializeField] private int _monsterNumber;

        private int _currentMonsterNumber;
        #endregion
    }
}
