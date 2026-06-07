using Levels.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Title.Runtime
{
    public class TitleMenu : MonoBehaviour
    {
        #region Main API

        public void StartGame()
        {
            Debug.Log("Start");
            LevelManager.Load(_level1);
        }
        public void LoadCredits()
        {
            Debug.Log("Credit");
            LevelManager.Load(_credits);
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        public void LoadTitleMenu()
        {
            LevelManager.Load(_titleMenu);
        }

#endregion


        #region Private and Protected

        [SerializeField] private LevelData _level1;
        [SerializeField] private LevelData _credits;
        [SerializeField] private LevelData _titleMenu;

        #endregion
    }
}