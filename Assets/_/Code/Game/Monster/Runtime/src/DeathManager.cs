using UnityEngine;

namespace Monster.Runtime
{
    public class DeathManager : MonoBehaviour
    {
        
        #region Main API

        public void Kill()
        {
            Instantiate(_pilPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }

        #endregion


        #region Private and Protected

        [SerializeField] private GameObject _pilPrefab;

        #endregion
    }
}
