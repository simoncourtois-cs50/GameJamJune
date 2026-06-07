using UnityEditor.Toolbars;

namespace Levels.Editor
{
    public class LevelFactoryToolbar
    {
        [MainToolbarElement(nameof(LevelFactory), defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarButton ShowWindow()
        {
            MainToolbarContent content = new MainToolbarContent("Level Factory", "Create and update levels: folder structure, scenes, LevelData and Build Settings configuration.");

            return new MainToolbarButton(content, OpenWindow);
        }

        private static void OpenWindow()
        {
            LevelFactory.ShowWindow();
        }
    }
}