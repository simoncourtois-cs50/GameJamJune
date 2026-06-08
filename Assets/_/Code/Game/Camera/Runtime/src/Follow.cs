using UnityEngine;

namespace Camera.Runtime
{
    public class Follow : MonoBehaviour
    {
        #region Public
        public Vector3 m_veloCity = Vector3.zero;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (!gameObject.TryGetComponent<Transform>(out _cameraTransform)) return;
        }

        private void LateUpdate()
        {
            FollowCameraBoxPosition();
        }

        #endregion


        #region Main API

        private void FollowCameraBoxPosition()
        {
            Vector3 _targetPosition = _cameraBox.position + _offset;
            Vector3 _originPosition = _cameraTransform.position;

            Vector3 currentPosition = Vector3.SmoothDamp(_originPosition, _targetPosition, ref m_veloCity, _smoothSpeed);
            _cameraTransform.position = currentPosition;
        }

        #endregion


        #region Private and Protected

        [SerializeField] private Transform _cameraBox;
        private Transform _cameraTransform;

        [SerializeField] private float _smoothSpeed;
        private Vector3 _offset = new Vector3(0, 0, -10f);
        #endregion
    }
}
