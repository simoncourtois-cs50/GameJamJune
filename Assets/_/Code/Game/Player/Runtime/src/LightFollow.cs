using UnityEngine;

namespace Player.Runtime
{
    public class LightFollow : MonoBehaviour
    {
        #region

        private void Update()
        {
            Follow();
        }

        #endregion


        #region Main API

        private void Follow()
        {
            transform.position = _player.position;
        }

        #endregion


        #region Private and Protected

        [SerializeField] private Transform _player;

        #endregion
    }
}
