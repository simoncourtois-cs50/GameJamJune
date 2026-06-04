using Levels.Runtime;
using UnityEngine;

namespace Title.Runtime
{
    public class TitleMenu : MonoBehaviour
    {
        #region Main API

        public void StartGame()
        {
            LevelManager.Load(_level1);
        }

        #endregion


        #region Private and Protected

        [SerializeField] private LevelData _level1;

        #endregion
    }
}