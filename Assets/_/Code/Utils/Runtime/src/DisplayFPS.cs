using TMPro;
using UnityEngine;

namespace Utils.Runtime
{
    public class DisplayFPS : MonoBehaviour
    {

        #region Unity API

        private void Update()
        {
            UpdateText();
        }

        #endregion


        #region Main API

        private void Awake()
        {
            if (!TryGetComponent(out _text)) return;
        }
        private int GetFPS()
        {
            Debug.Log((int)1/Time.deltaTime);
            return Time.captureFramerate;
        }

        private void UpdateText()
        {
            _text.text = GetFPS().ToString();
        }

        #endregion


        #region Private and Protected

        private TMP_Text _text;

        #endregion
    }
}