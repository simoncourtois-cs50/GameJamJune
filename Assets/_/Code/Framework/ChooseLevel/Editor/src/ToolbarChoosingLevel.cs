using UnityEditor;
using UnityEditor.Toolbars;

namespace ChooseLevel.runtime
{
        public class ToolbarChoosingLevel : EditorWindow
        {
            [MainToolbarElement("Choosing Level", defaultDockPosition = MainToolbarDockPosition.Left)]
            public static MainToolbarButton ShowWindow()
            {
                MainToolbarContent content = new MainToolbarContent("Choosing Level", "Create Feature & Assembly Definition");
			
                return new MainToolbarButton(content, OpenWindow);
            }

            private static void OpenWindow()
            {
                ChoosingLevel.ShowWindow();
            }
        }
    }
