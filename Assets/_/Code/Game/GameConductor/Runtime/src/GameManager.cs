
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

        public void Reset()
        {
            _health.Reset();
            _victoryScreen.SetActive(false);
            _gameoverScreen.SetActive(false);
            _currentMonsterNumber = _monsterNumber;
        }

        #endregion


        #region Private and Protected

        [Header("References")]
        [SerializeField] private GameObject _gameoverScreen;
        [SerializeField] private GameObject _victoryScreen;
        [SerializeField] private ManageTarget _player;
        [SerializeField] private EntityHealth _health;    
        [Header("Game Variables")]
        [SerializeField] private int _monsterNumber;

        private int _currentMonsterNumber;
        #endregion
    }
}
