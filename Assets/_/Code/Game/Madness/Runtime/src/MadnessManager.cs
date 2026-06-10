using CameraManager.Runtime;
using Player.Runtime;
using UnityEngine;

namespace Madness.Runtime
{
    public class MadnessManager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _flickerInterval = 0;
            _player.OnPillPicked += IncrementDrugLevel;
        }

        private void Update()
        {
            ChangeFlickerInterval();
            ChangeAimDrunkness();
            ChangeCameraShaking();
        }

        #endregion


        #region Main API

        private void ChangeFlickerInterval()
        {
            if(_currentDrugLevel >= _drugLevel4)
            {
                _flickerInterval = _flicker4;
            }
            else if (_currentDrugLevel >= _drugLevel3)
            {
                _flickerInterval = _flicker3;
            }
            else if (_currentDrugLevel >= _drugLevel2)
            {
                _flickerInterval = _flicker2;
            }
            else if(_currentDrugLevel >= _drugLevel1)
            {
                _flickerInterval = _flicker1;
            }
            
            _light.SetFlickerInterval(_flickerInterval);
        }

        private void ChangeAimDrunkness()
        {
            if (_currentDrugLevel >= _drugLevel4)
            {
                _drunkness = _drunkness4;
            }
            else if (_currentDrugLevel >= _drugLevel3)
            {
                _drunkness = _drunkness3;
            }
            else if (_currentDrugLevel >= _drugLevel2)
            {
                _drunkness = _drunkness2;
            }
            else if (_currentDrugLevel >= _drugLevel1)
            {
                _drunkness = _drunkness1;
            }
            _player.SetDrunkness(_drunkness);
        }
        private void ChangeCameraShaking()
        {
            if (_currentDrugLevel >= _drugLevel4)
            {
                _currentCameraInterval = _interval4;
                _currentCameraAmplitude = _amplitude4;
            }
            else if (_currentDrugLevel >= _drugLevel3)
            {
                _currentCameraInterval = _interval3;
                _currentCameraAmplitude = _amplitude3;
            }
            else if (_currentDrugLevel >= _drugLevel2)
            {
                _currentCameraInterval = _interval2;
                _currentCameraAmplitude = _amplitude2;
            }
            else if (_currentDrugLevel >= _drugLevel1)
            {
                _currentCameraInterval = _interval1;
                _currentCameraAmplitude = _amplitude1;
            }

            _camera.SetShake(_currentCameraInterval, _currentCameraAmplitude);

        }

        private void IncrementDrugLevel()
        {
            _currentDrugLevel++;
        }

        public void Reset()
        {
            _currentDrugLevel = 0;
            
            _flickerInterval = 0;
            _drunkness = 0;
            _currentCameraInterval = 0;
            _currentCameraAmplitude = 0;
            
            _camera.SetShake(0, 0);
            _player.SetDrunkness(0);
            _light.SetFlickerInterval(0);
            Debug.Log("reset");
        }
        #endregion


        #region Private and Protected

        [SerializeField] private ManageTarget _player;
        [SerializeField] private Follow _camera;
        [SerializeField] private Flicker _light;
        [Header("Drug Levels")]
        [SerializeField] private int _drugLevel1;
        [SerializeField] private int _drugLevel2;
        [SerializeField] private int _drugLevel3;
        [SerializeField] private int _drugLevel4;

        [Header("Flicker Intervals Levels")]
        [SerializeField] private float _flicker1;
        [SerializeField] private float _flicker2;
        [SerializeField] private float _flicker3;
        [SerializeField] private float _flicker4;

        [Header("Drunkness Levels")]
        [SerializeField] private float _drunkness1;
        [SerializeField] private float _drunkness2;
        [SerializeField] private float _drunkness3;
        [SerializeField] private float _drunkness4;

        [Header("Camera Blinking")]
        [SerializeField] private float _amplitude1;
        [SerializeField] private float _interval1;

        [SerializeField] private float _amplitude2;
        [SerializeField] private float _interval2;

        [SerializeField] private float _amplitude3;
        [SerializeField] private float _interval3;

        [SerializeField] private float _amplitude4;
        [SerializeField] private float _interval4;

        private float _flickerInterval;
        private float _drunkness;
        private int _currentDrugLevel;
        private float _currentCameraAmplitude;
        private float _currentCameraInterval;


        #endregion
    }
}