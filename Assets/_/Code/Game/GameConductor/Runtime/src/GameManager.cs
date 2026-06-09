using System.Reflection;
using Player.Runtime;
using UnityEngine;

namespace GameConductor.Runtime
{
    public class GameManager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _currentMonsterNumber = _monsterNumber;
        }

        private void Start()
        {
            _player.OnMonsterKill += DecrementKillCount;
        }
        #endregion
        
        
        #region Main API

        private void DecrementKillCount()
        {
            _currentMonsterNumber--;
            CheckVictory();
        }

        private void CheckVictory()
        {
            if (_currentMonsterNumber > 0) return;
            _victoryScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
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
