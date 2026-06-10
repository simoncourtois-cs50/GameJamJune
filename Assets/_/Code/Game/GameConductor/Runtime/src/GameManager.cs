
using Player.Runtime;
using Timer.Runtime;
using TMPro;
using UnityEngine;

namespace GameConductor.Runtime
{
    public class GameManager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _currentMonsterNumber = _monsterNumber;
            UpdateCounter();
        }

        private void Start()
        {
            _player.OnMonsterKill += DecrementKillCount;
            _health.OnDeath += HandleEndTimer;
            TimeManager.Instance.OnEnd += HandleEndTimer;
        }

        private void Update()
        {
            UpdateTimer();
        }

        #endregion
        
        
        #region Main API

        private void DecrementKillCount()
        {
            _currentMonsterNumber--;
            UpdateCounter();
            CheckVictory();
        }

        private void CheckVictory()
        {
            if (_currentMonsterNumber > 0) return;
            _victoryScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void HandleEndTimer()
        {
            TimeManager.Instance.PauseTimer();
            _gameoverScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void Reset()
        {
            TimeManager.Instance.ResetTimer();
            TimeManager.Instance.PlayTimer();
            _health.Reset();
            _victoryScreen.SetActive(false);
            _gameoverScreen.SetActive(false);
            _currentMonsterNumber = _monsterNumber;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            _monsterCounter.text = _currentMonsterNumber.ToString();
        }

        private void UpdateTimer()
        {
            float time = Mathf.Max(0, TimeManager.Instance.m_currentTime);
            int minute = (int)time / 60;
            int second = (int)time % 60;
            string formatedTimer = $"{minute:D2}:{second:D2}";
            _timer.text = formatedTimer;
        }

        #endregion

        #region Private and Protected

        [Header("References")]
        [SerializeField] private GameObject _gameoverScreen;
        [SerializeField] private GameObject _victoryScreen;
        [SerializeField] private TMP_Text _monsterCounter;
        [SerializeField] private TMP_Text _timer;
        [SerializeField] private ManageTarget _player;
        [SerializeField] private EntityHealth _health;    
        [Header("Game Variables")]
        [SerializeField] private int _monsterNumber;

        private int _currentMonsterNumber;
        #endregion
    }
}
