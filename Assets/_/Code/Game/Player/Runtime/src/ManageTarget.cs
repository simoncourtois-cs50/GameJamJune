using UnityEngine.InputSystem;
using UnityEngine;
using Monster.Runtime;
using Item.Runtime;
using Loader.Runtime;

namespace Player.Runtime
{
    public class ManageTarget : MonoBehaviour
    {
        #region

        public bool _isDrunk;

        #endregion


        #region Unity API

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            transform.position = Vector3.zero;
            _playerHealth = GetComponent<EntityHealth>();
            RegisterBackgroundBounds();
            
        }
        private void Update()
        {
            DetectClick();
            FollowMouse();

            if (_isDrunk)
            {
                DrunkAim();
            }
        }

        #endregion

        
        #region Main API

        private void FollowMouse()
        {
            Vector3 delta = Pointer.current.delta.ReadValue();
            transform.position += _mouseSensitivity * Time.deltaTime * delta;
            ClampPositions();
        }

        private void DetectClick()
        {
            if (_clickAction.action.WasPressedThisFrame())
            {
                Collider2D hit = Physics2D.OverlapPoint(transform.position, _clickLayer.value);
                if (!hit) return;

                if(hit.gameObject.TryGetComponent<DeathManager>(out _monster))
                {
                    _monster.Kill();
                }
                else if(hit.gameObject.TryGetComponent<Pill>(out _pill))
                {
                    _pill.PickUp();
                    _playerHealth.TakePill();
                }
                else if(hit.gameObject.TryGetComponent(out _navigationArrow))
                {
                    _navigationArrow.LoadNextRoom();
                }
            }
        }
        private void ClampPositions()
        {
            float xPos = transform.position.x;
            float yPos = transform.position.y;
            float xClamped = Mathf.Clamp(xPos, _xMinBound, _xMaxBound);
            float yClamped = Mathf.Clamp(yPos, _yMinBound, _yMaxBound);
            transform.position = new Vector3(xClamped, yClamped, 0);
        }
        
        private void RegisterBackgroundBounds()
        {
            _xMaxBound = _backgroundCollider.bounds.max.x;
            _xMinBound = _backgroundCollider.bounds.min.x;
            _yMaxBound = _backgroundCollider.bounds.max.y;
            _yMinBound = _backgroundCollider.bounds.min.y;
        }

        private void DrunkAim()
        {
            _perlinTimer += Time.deltaTime;

            float drunkX = (Mathf.PerlinNoise(_perlinTimer, 0f) - 0.5f) * _drunkIntensity;
            float drunkY = (Mathf.PerlinNoise( 0f, _perlinTimer) - 0.5f) * _drunkIntensity;

            Vector3 drunkOffset = new Vector3(drunkX, drunkY, 0f);

            transform.position += drunkOffset;
        }
        
        #endregion


        #region Private and Protected

        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionReference _clickAction;
        [SerializeField] private LayerMask _clickLayer;
        [SerializeField] private Collider2D _backgroundCollider;
        [SerializeField] private float _mouseSensitivity;
        [SerializeField] private float _drunkIntensity;

        private EntityHealth _playerHealth;
        private DeathManager _monster;
        private Pill _pill;
        private SwitchRoom _navigationArrow;
        
        private float _xMaxBound;
        private float _xMinBound;
        private float _yMaxBound;
        private float _yMinBound;

        private float _perlinTimer;

        #endregion
    }
}
