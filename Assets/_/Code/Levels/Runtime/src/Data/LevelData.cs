using System.Collections.Generic;
using UnityEngine;

namespace Levels.Runtime
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Framework/Level")]
    public class LevelData : ScriptableObject
    {
        #region Publics

        [Tooltip("All scenes that compose this level.")]
        public List<SceneReference> m_scenes = new List<SceneReference>();

        [Tooltip("The scene that will be set as the active scene after loading. Must be part of m_scenes.")]
        public SceneReference m_activeScene;

        #endregion


        #region Unity API

        private void OnValidate()
        {
#if UNITY_EDITOR
            for (int i = 0; i < m_scenes.Count; i++)
            {
                m_scenes[i]?.Sync();
            }

            m_activeScene?.Sync();
#endif
        }

        #endregion
    }
}