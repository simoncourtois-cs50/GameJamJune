using UnityEngine.InputSystem;
using UnityEngine;
using Monster.Runtime;
using Item.Runtime;

namespace Player.Runtime
{
    public class ManageTarget : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            Cursor.visible = false;
            _playerHealth = GetComponent<EntityHealth>();

        }
        private void Update()
        {
            DetectClick();
        }
        private void LateUpdate()
        {
            FollowMouse();
        }

        #endregion

        #region Main API

        private Vector3 GetMousePosition()
        {
            Vector3 mousePosition = Pointer.current.position.ReadValue();
            mousePosition.z = Mathf.Abs(_camera.transform.position.z);

            Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(mousePosition);
            mouseWorldPosition.z = 0f;
            return mouseWorldPosition;
        }

        private void FollowMouse()
        {
            transform.position = GetMousePosition();
        }

        private void DetectClick()
        {
            if (_clickAction.action.WasPressedThisFrame())
            {
                Vector3 mousePosition = GetMousePosition();
                Collider2D hit = Physics2D.OverlapPoint(mousePosition, _clickLayer.value);
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
            }
        }
        #endregion


        #region Private and Protected

        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionReference _clickAction;
        [SerializeField] private LayerMask _clickLayer;

        private EntityHealth _playerHealth;
        private DeathManager _monster;
        private Pill _pill;
        #endregion
    }
}
