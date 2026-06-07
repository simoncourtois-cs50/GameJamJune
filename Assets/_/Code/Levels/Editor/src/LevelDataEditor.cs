using Levels.Runtime;
using UnityEditor;
using UnityEngine;

namespace Levels.Editor
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        #region Main API

        public override void OnInspectorGUI()
        {
            LevelData levelData = (LevelData)target;

            serializedObject.Update();

            EditorGUILayout.LabelField("Scenes", EditorStyles.boldLabel);

            if (levelData.m_scenes.Count == 0)
            {
                EditorGUILayout.HelpBox("No scenes assigned.", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < levelData.m_scenes.Count; i++)
                {
                    var sceneRef = levelData.m_scenes[i];

                    bool isActive = levelData.m_activeScene != null
                        && levelData.m_activeScene.ScenePath == sceneRef.ScenePath
                        && !string.IsNullOrEmpty(sceneRef.ScenePath);

                    EditorGUILayout.BeginHorizontal();

                    bool newIsActive = EditorGUILayout.Toggle(isActive, GUILayout.Width(16));
                    if (newIsActive && !isActive)
                    {
                        levelData.m_activeScene = sceneRef;
                        EditorUtility.SetDirty(levelData);
                    }

                    string label = isActive ? "★" : $"{i + 1}.";
                    EditorGUILayout.LabelField(label, GUILayout.Width(24));

                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(sceneRef.m_sceneAsset, typeof(SceneAsset), false);
                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField("Active Scene", EditorStyles.boldLabel);

            if (levelData.m_activeScene != null && levelData.m_activeScene.m_sceneAsset != null)
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField(levelData.m_activeScene.m_sceneAsset, typeof(SceneAsset), false);
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.HelpBox("No active scene set. The first scene will be used as fallback.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}