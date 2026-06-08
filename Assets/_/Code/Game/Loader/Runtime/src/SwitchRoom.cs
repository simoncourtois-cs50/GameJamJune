using Levels.Runtime;
using UnityEngine;

namespace Loader.Runtime
{
    public class SwitchRoom : MonoBehaviour
    {   
        #region MyRegion

        public void LoadNextRoom()
        {
            LevelManager.Load(_nextRoom);
        }

        #endregion


        #region

        [SerializeField] private LevelData _nextRoom;

        #endregion
    }
}
