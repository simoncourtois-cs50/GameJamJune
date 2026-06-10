using GameConductor.Runtime;
using Levels.Runtime;
using Madness.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Title.Runtime
{
    public class RestartMenu : MonoBehaviour
    {
        #region Main API

        
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        public void HardRestartGame()
        {

            LevelManager.Load(_titleMenu);
        }

        #endregion

        #region Private and Protected

        [SerializeField] private LevelData _titleMenu;

        #endregion
    }
}
