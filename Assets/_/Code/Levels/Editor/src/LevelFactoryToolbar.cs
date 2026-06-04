using UnityEditor.Toolbars;

namespace Levels.Editor
{
    public class LevelFactoryToolbar
    {
        [MainToolbarElement(nameof(LevelFactory), defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarButton ShowWindow()
        {
            MainToolbarContent content = new MainToolbarContent("Create Level", "Create LevelData assets with associated scenes and Build Settings configuration");

            return new MainToolbarButton(content, OpenWindow);
        }

        private static void OpenWindow()
        {
            LevelFactory.ShowWindow();
        }
    }
}