using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils.Runtime
{
    public class DisplayFPS : MonoBehaviour
    {

        #region Unity API

        private void Update()
        {
#if UNITY_EDITOR
            CheckInput();
            EmptyText();
            if(isTimeToDisplay() && _isVisible) UpdateText();
#endif
        }

        #endregion


        #region Main API

        private void Awake()
        {
            if (!TryGetComponent(out _text)) return;
        }
        private int GetFPS()
        {
            return (int)(1 / Time.deltaTime);
        }

        private void UpdateText()
        {
            int FPS = GetFPS();

            Color color = Color.white;

            if(FPS <= 30)
            {
                color = Color.red;
            }
            if(FPS <= 60)
            {
                color = Color.yellow;
            }
            _text.color = color;
            _text.text = GetFPS().ToString();
        }

        private bool isTimeToDisplay()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime < 1) return false;
            _currentTime = 0;
            return true;
        }

        private void ToggleFPSCounter()
        {
            if (_isVisible)
            {
                _isVisible = false;
                return;
            }
            _isVisible = true;
        }
        private void EmptyText()
        {
            if (!_isVisible) _text.text = "";
        }

        private void CheckInput()
        {
            if (actionReference.action.WasPressedThisFrame())
            {
                ToggleFPSCounter();
            }
        }

        #endregion


        #region Private and Protected

        private TMP_Text _text;
        private float _currentTime;
        private bool _isVisible;
        [SerializeField] private InputActionReference actionReference;
        #endregion
    }
}