using Player.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Runtime
{
    public class SanityBar : MonoBehaviour
    {
        #region UnityAPI

        private void Awake()
        {
            _playerSanity.OnHPChangeNormalized += UpdateSanityBar;
        }

        #endregion


        #region Main API

        private void UpdateSanityBar(float sanity)
        {
            _sanityBar.fillAmount = sanity;
        }

        #endregion


        #region Private and Protected

        [SerializeField] private Image _sanityBar;
        [SerializeField] private EntityHealth _playerSanity;
        
        #endregion
    }
}
