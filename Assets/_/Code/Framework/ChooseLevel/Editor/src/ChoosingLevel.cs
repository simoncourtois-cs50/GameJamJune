using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Levels.Runtime;

namespace ChooseLevel.runtime
{
    public class ChoosingLevel : EditorWindow
    {
        #region Private and Protected

        private List<LevelData> _levels = new List<LevelData>();
        private Vector2 _scrollPos;
        
        private const string BASE_SCENE_PATH = "Assets/_/DataBase/Scenes/Global.unity"; //  Path Of Where to look

        #endregion


        #region Main API

        public static void ShowWindow()
        {
            ChoosingLevel window = GetWindow<ChoosingLevel>("Choosing Level");
            window.minSize = new Vector2(200, 150);
            window.LoadAllLevelData();
        }

        #endregion


        #region Unity API

        private void OnGUI()
        {
            GUILayout.Label("Available Levels", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _levels.RemoveAll(l => l == null);

            if (_levels.Count == 0)
            {
                EditorGUILayout.HelpBox("No LevelData found in the project.", MessageType.Info);
                if (GUILayout.Button("Refresh")) LoadAllLevelData();
                return;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach (LevelData level in _levels)
            {
                if (level == null) continue;

                float textWidth = EditorStyles.label.CalcSize(new GUIContent(level.name)).x;

                EditorGUILayout.BeginHorizontal("box", GUILayout.Width(textWidth));

                GUILayout.Label(level.name, GUILayout.ExpandWidth(false));

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                if (GUILayout.Button("Load", GUILayout.Width(60)))
                    LoadLevelInEditor(level);
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                    EditorGUIUtility.PingObject(level);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh List", GUILayout.Height(36))) LoadAllLevelData();
        }

        #endregion


        #region Private and Protected

        private void LoadAllLevelData()
        {
            _levels.Clear();
            string[] guids = AssetDatabase.FindAssets("t:LevelData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
                if (level != null) _levels.Add(level);
            }
        }

        private void LoadLevelInEditor(LevelData level)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[ChoosingLevel] Cannot load a level while in Play Mode.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
            if (level == null) return;
            if (level.m_scenes.Count == 0)
            {
                Debug.LogWarning($"[ChoosingLevel] {level.name} has no scenes.");
                return;
            }

            EditorApplication.delayCall += () => LoadLevelDelayed(level);
        }

        private void LoadLevelDelayed(LevelData level)
        {
            SceneAsset baseScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BASE_SCENE_PATH);

            if (baseScene == null)
            {
                Debug.LogWarning($"[ChoosingLevel] Base scene not found at: {BASE_SCENE_PATH}. Creating it...");

                string directory = System.IO.Path.GetDirectoryName(BASE_SCENE_PATH).Replace("\\", "/");
                EnsureFolderExists(directory);

                var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                EditorSceneManager.SaveScene(newScene, BASE_SCENE_PATH);
                AssetDatabase.Refresh();

                Debug.Log($"[ChoosingLevel] Scene 'Global' created at: {BASE_SCENE_PATH}");
            }
            else
            {
                EditorSceneManager.OpenScene(BASE_SCENE_PATH, OpenSceneMode.Single);
            }

            string activeScenePath = (level.m_activeScene != null && !string.IsNullOrEmpty(level.m_activeScene.ScenePath))
                ? level.m_activeScene.ScenePath
                : level.m_scenes[0].ScenePath;

            foreach (SceneReference sceneRef in level.m_scenes)
            {
                if (!string.IsNullOrEmpty(sceneRef.ScenePath))
                    EditorSceneManager.OpenScene(sceneRef.ScenePath, OpenSceneMode.Additive);
            }

            UnityEngine.SceneManagement.Scene active = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(activeScenePath);
            if (active.IsValid())
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(active);
            else
                Debug.LogWarning($"[ChoosingLevel] Could not set active scene: {activeScenePath}");
        }
        private static void EnsureFolderExists(string folderPath)
        {
            string[] parts = folderPath.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                    AssetDatabase.Refresh();
                }
                current = next;
            }
        }

        #endregion
    }
}