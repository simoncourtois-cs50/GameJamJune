using System;
using UnityEditor;
using UnityEngine;

namespace Levels.Runtime
{
    [Serializable]
    public class SceneReference : MonoBehaviour
    {
        #region Publics

#if UNITY_EDITOR
        public SceneAsset m_sceneAsset;
#endif

        public string ScenePath => _scenePath;
        public string SceneName => _sceneName;

        #endregion


        #region utils

        public void Sync()
        {
#if UNITY_EDITOR
            _scenePath = m_sceneAsset ? AssetDatabase.GetAssetPath(m_sceneAsset) : string.Empty;
            _sceneName = m_sceneAsset ? m_sceneAsset.name : string.Empty;
#endif
        }

        #endregion

        #region Private and Protected

        private string _scenePath;
        private string _sceneName;

        #endregion
    }
}
