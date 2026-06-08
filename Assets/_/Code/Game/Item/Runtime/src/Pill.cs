using UnityEngine;

namespace Item.Runtime
{
    public class Pill : MonoBehaviour
    {
        public void PickUp()
        {
            Destroy(this.gameObject);
        }
    }
}
