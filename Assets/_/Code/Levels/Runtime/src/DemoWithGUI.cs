using UnityEngine;

namespace Levels.Runtime
{
    public class DemoWithGUI : MonoBehaviour
    {
        #region Publics

        public LevelData m_debugLevelOne;
        public LevelData m_debugLevelTwo;

        #endregion


        #region Unity APO

        public void OnGUI()
        {
            if (GUILayout.Button("Load Debug LevelOne"))
            {
                _ = LevelManager.Load(m_debugLevelOne);
            }
            if (GUILayout.Button("Load Debug LevelTwo"))
            {
                _ = LevelManager.Load(m_debugLevelTwo);
            }
        }

        #endregion
    }
}