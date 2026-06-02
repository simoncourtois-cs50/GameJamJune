using System.Collections.Generic;
using UnityEngine;

namespace Levels.Runtime
{
    [CreateAssetMenu(fileName = "NewLevel", menuName ="Framework/Level")]
    public class LevelData : ScriptableObject
    {
        #region Publics

        public List<SceneReference> m_scenes = new List<SceneReference>();

        #endregion


        #region Unity API

        private void OnValidate()
        {
#if UNITY_EDITOR
            for(int i = 0; i < m_scenes.Count; i++)
            {
                m_scenes[i]?.Sync();
            }
#endif
        }

        #endregion

    }
}
