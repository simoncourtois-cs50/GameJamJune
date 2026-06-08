using UnityEngine;

namespace CameraManager.Runtime
{
    public class BoxMotion : MonoBehaviour
    {
        #region Public

        public Vector3 m_velocity = Vector3.zero;

        #endregion


        #region Unity API

        private void Awake()
        {
            if (!gameObject.TryGetComponent(out _collider)) return;
            if (!gameObject.TryGetComponent(out _boxTransform)) return;

            _collider.size = _camBoxSize;
        }

        private void LateUpdate()
        {
            PushBoxRight();
            PushBoxLeft();
            PushBoxUp();
            PushBoxDown();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _camBoxSize);
        }

        #endregion


        #region Main API

        private void PushBoxRight()
        {
            float xDistanceFromRight = _playerTransform.position.x - _collider.bounds.max.x;

            if (xDistanceFromRight < 0) return;

            Vector3 position = _boxTransform.position;
            position.x += xDistanceFromRight;
            _boxTransform.position = position;
        }

        private void PushBoxLeft()
        {
            float xDistanceFromLeft = _playerTransform.position.x - _collider.bounds.min.x;

            if (xDistanceFromLeft > 0) return;

            Vector3 position = _boxTransform.position;
            position.x += xDistanceFromLeft;
            _boxTransform.position = position;
        }

        private void PushBoxUp()
        {
            float xDistanceFromUp = _playerTransform.position.y - _collider.bounds.max.y;

            if (xDistanceFromUp < 0) return;

            Vector3 position = _boxTransform.position;
            position.y += xDistanceFromUp;
            _boxTransform.position = position;
        }

        private void PushBoxDown()
        {
            float xDistanceFromDown = _playerTransform.position.y - _collider.bounds.min.y;

            if (xDistanceFromDown > 0) return;

            Vector3 position = _boxTransform.position;
            position.y += xDistanceFromDown;
            _boxTransform.position = position;
        }

        

        private void HandleOnGrounded()
        {
            Vector3 _originPosition = _boxTransform.position;
            Vector3 targetPosition = _originPosition;
            targetPosition.y = _playerTransform.position.y;
            Vector3 currentPosition = Vector3.SmoothDamp(_originPosition, targetPosition, ref m_velocity, _smoothSpeed);
            _boxTransform.position = currentPosition;
        }
        

        #endregion


        #region Private and Protected

        private BoxCollider2D _collider;
        private Transform _boxTransform;
        private float _smoothSpeed = 0.4f;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Vector3 _camBoxSize;
        
        
        
        
        #endregion
    }
}
