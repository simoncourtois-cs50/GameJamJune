using UnityEngine.InputSystem;
using UnityEngine;

namespace Player.Runtime
{
    public class MoveTarget : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            Cursor.visible = false;
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
            return mouseWorldPosition;

        }

        private void FollowMouse()
        {
            transform.position = GetMousePosition();
        }

        #endregion


        #region Private and Protected

        [SerializeField] private Camera _camera;
        #endregion
    }
}
