using UnityEngine;

namespace CameraManager.Runtime
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
            RegisterBackgroundBounds();
            Camera camera = Camera.main;
            _halfHeight = camera.orthographicSize;
            _halfwidth = camera.aspect * _halfHeight;
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
            ClampPositions();
        }

        private void ClampPositions()
        {
            float xPos = transform.position.x;
            float yPos = transform.position.y;
            float xClamped = Mathf.Clamp(xPos, _xMinBound + _halfwidth, _xMaxBound - _halfwidth);
            float yClamped = Mathf.Clamp(yPos, _yMinBound + _halfHeight, _yMaxBound - _halfHeight);
            transform.position = new Vector3(xClamped, yClamped, -10f);
        }
        
        private void RegisterBackgroundBounds()
        {
            _xMaxBound = _backgroundCollider.bounds.max.x;
            _xMinBound = _backgroundCollider.bounds.min.x;
            _yMaxBound = _backgroundCollider.bounds.max.y;
            _yMinBound = _backgroundCollider.bounds.min.y;
        }
        
        #endregion


        #region Private and Protected

        [SerializeField] private Transform _cameraBox;
        private Transform _cameraTransform;

        [SerializeField] private float _smoothSpeed;
        private Vector3 _offset = new Vector3(0, 0, -10f);
        
        [SerializeField] private Collider2D _backgroundCollider;

        private float _halfHeight;
        private float _halfwidth;
        
        private float _xMaxBound;
        private float _xMinBound;
        private float _yMaxBound;
        private float _yMinBound;
        
        #endregion
    }
}
