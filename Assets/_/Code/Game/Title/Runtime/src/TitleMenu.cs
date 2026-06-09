using GameConductor.Runtime;
using Levels.Runtime;
using Madness.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Title.Runtime
{
    public class TitleMenu : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _gameManager = _gameManagerRef.gameObject.GetComponent<GameManager>();
            _madnessManager = _madnessManagerReference.gameObject.GetComponent<MadnessManager>();
        }

        #endregion
        
        
        #region Main API

        public void StartGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _gameManager.Reset();
            _madnessManager.Reset();
            LevelManager.Load(_level1);
        }
        public void LoadCredits()
        {
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
        [SerializeField] private GuidReference _gameManagerRef;
        [SerializeField] private GuidReference _madnessManagerReference;
        private GameManager _gameManager;
        private MadnessManager _madnessManager;
        #endregion
    }
}