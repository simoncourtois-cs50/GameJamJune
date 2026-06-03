using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Levels.Runtime
{
    public class LevelManager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            Load(_titleScreen);
        }

        #endregion


        #region Main API

        public static async Task Load(LevelData level)
        {
            if (level == null) return;
            if (level.m_scenes.Count == 0) return;

            if (_currentLevel != null) Unload(_currentLevel);

            _currentLevel = level;

            List<SceneReference> scenes = level.m_scenes;

            for (int i = 0; i < scenes.Count; i++)
            {
                var path = scenes[i].ScenePath;
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
                while (!asyncOperation.isDone) await Task.Yield();
            }
        }

        public static async Task Unload(LevelData level)
        {
            if (level == null) return;
            if (level.m_scenes.Count == 0) return;

            List<SceneReference> scenes = level.m_scenes;

            for (int i = 0; i < scenes.Count; i++)
            {
                var path = scenes[i].ScenePath;
                AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(path);
                while (!asyncOperation.isDone) await Task.Yield();
            }
        }

        #endregion


        #region Private and Protected

        private static LevelData _currentLevel;
        [SerializeField] LevelData _titleScreen;
        #endregion
    }
}