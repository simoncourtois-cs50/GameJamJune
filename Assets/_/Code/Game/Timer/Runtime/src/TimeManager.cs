using System;
using UnityEngine;

namespace Timer.Runtime
{
    public class TimeManager : MonoBehaviour
    {
         #region Public

        public bool m_isPlaying { get; private set; }
        public float m_currentTime { get; private set; }

        public event Action OnPause;
        public event Action OnPlay;
        public event Action OnEnd;
        public static TimeManager Instance { get; private set; }

        #endregion


        #region Unity API

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }
            m_currentTime = _timerValue;
        }

        private void Update()
        {
            if (m_isPlaying) m_currentTime -= Time.deltaTime;

            CheckEndGame();
        }

        #endregion


        #region Main API

        public void PauseTimer()
        {
            m_isPlaying = false;
            OnPause?.Invoke();
        }

        public void PlayTimer()
        {
            m_isPlaying = true;
            
            OnPlay?.Invoke();
        }

        public void ResetTimer()
        {
            m_currentTime = _timerValue;
        }

        private void CheckEndGame()
        {
            if (m_currentTime > 0) return;
            ResetTimer();
            PauseTimer();
            OnEnd?.Invoke();
        }

        #endregion

        #region

        [SerializeField] private float _timerValue;
        

        #endregion
    }
}