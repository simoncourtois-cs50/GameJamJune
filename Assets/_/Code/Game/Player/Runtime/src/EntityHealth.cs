using System;
using UnityEngine;

namespace Player.Runtime
{
    public class EntityHealth : MonoBehaviour
    {

        #region Public

        public event Action OnDamage;
        public event Action OnHeal;
        public event Action<float> OnHPChange;

        public event Action<float> OnHPChangeNormalized;

        public event Action OnDeath;

        #endregion

        #region Unity API

        private void Awake()
        {
            _currentMadness = 0;
            OnHPChange?.Invoke(_currentMadness);
            OnHPChangeNormalized?.Invoke(_currentMadness / _maxMadness);
        }
        private void Update()
        {
            LooseHealth();
        }

        #endregion


        #region Main API

        public void Heal(float healPoints)
        {
            if (_isDead || _isInvincible) return;

            _currentMadness -= healPoints;
            HandleHealthChange();
            OnHeal?.Invoke();

        }
        public void TakePill()
        {
            Heal(_pillRecoveryValue);
        }
        public void TakeMadness(float damagePoints)
        {
            if (!_isDead)
            {
                _currentMadness += damagePoints;
                HandleHealthChange();
                OnDamage?.Invoke();
            }

            if (_currentMadness >= _maxMadness)
            {
                OnDeath?.Invoke();
                _isDead = true;
            }
        }

        private void HandleHealthChange()
        {
            Mathf.Clamp(_currentMadness, 0, _maxMadness);
            float normalizedHealth = _currentMadness / _maxMadness;

            OnHPChange?.Invoke(_currentMadness);
            OnHPChangeNormalized?.Invoke(normalizedHealth);
        }

        private void LooseHealth()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime < 1) return;

            TakeMadness(_madnessPerSecond);
            _currentTime = 0;
        }

        #endregion


        #region Private and Protected

        [SerializeField] private float _maxMadness;
        [SerializeField] private float _madnessPerSecond;
        [SerializeField] private float _pillRecoveryValue;

        private float _currentTime;
        private float _currentMadness;
        private bool _isDead;
        private bool _isInvincible;

        #endregion
    }
}
