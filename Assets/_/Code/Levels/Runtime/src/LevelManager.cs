using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Levels.Runtime
{
    public class LevelManager : MonoBehaviour
    {
        #region Unity API

        private async void Awake()
        {
            await InitializeFromOpenScenes();
            if (_manualChooseLevel) return;
            Load(_startMenu);
        }

        #endregion


        #region Public API

        public static async Task Load(LevelData level)
        {
            if (level == null) return;
            if (level.m_scenes.Count == 0) return;

            if (_currentLevel != null) await Unload(_currentLevel);

            _currentLevel = level;

            List<SceneReference> scenes = level.m_scenes;

            string activeScenePath = (level.m_activeScene != null && !string.IsNullOrEmpty(level.m_activeScene.ScenePath))
                ? level.m_activeScene.ScenePath
                : scenes[0].ScenePath;

            for (int i = 0; i < scenes.Count; i++)
            {
                var path = scenes[i].ScenePath;
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
                while (!asyncOperation.isDone) await Task.Yield();

                if (path == activeScenePath) SceneManager.SetActiveScene(SceneManager.GetSceneByPath(path));
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
                if (asyncOperation != null) while (!asyncOperation.isDone) await Task.Yield();
            }
        }

        #endregion


        #region Main API

        private async Task InitializeFromOpenScenes()
        {
            List<Scene> openScenes = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.path != gameObject.scene.path)
                    openScenes.Add(scene);
            }

            LevelData matchedLevel = null;
            if (openScenes.Count > 0)
            {
                LevelData[] allLevels = Resources.FindObjectsOfTypeAll<LevelData>();
                foreach (LevelData levelData in allLevels)
                {
                    if (MatchesOpenScenes(levelData, openScenes))
                    {
                        matchedLevel = levelData;
                        break;
                    }
                }
            }

            if (matchedLevel != null) await Load(matchedLevel);

            foreach (Scene scene in openScenes)
            {
                if (!scene.isLoaded) continue;
                AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
                if (op != null) while (!op.isDone) await Task.Yield();
            }

            if (matchedLevel != null)
            {
                string activeScenePath = (matchedLevel.m_activeScene != null && !string.IsNullOrEmpty(matchedLevel.m_activeScene.ScenePath))
                    ? matchedLevel.m_activeScene.ScenePath
                    : matchedLevel.m_scenes[0].ScenePath;

                Scene activeScene = SceneManager.GetSceneByPath(activeScenePath);
                if (activeScene.isLoaded) SceneManager.SetActiveScene(activeScene);
            }
        }

        private static bool MatchesOpenScenes(LevelData levelData, List<Scene> openScenes)
        {
            if (levelData.m_scenes.Count == 0) return false;

            foreach (SceneReference sceneRef in levelData.m_scenes)
            {
                bool found = false;
                foreach (Scene scene in openScenes)
                {
                    if (scene.path == sceneRef.ScenePath)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) return false;
            }

            return true;
        }

        #endregion


        #region Private and Protected

        private static LevelData _currentLevel;

        [SerializeField] private LevelData _startMenu;
        [SerializeField] private bool _manualChooseLevel;

        #endregion
    }
}