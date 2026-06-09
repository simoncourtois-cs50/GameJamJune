using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeneratePrimitiveFromReference.Editor
{
    public class QuadFromSpriteGenerator : EditorWindow
    {
        private Sprite _sprite;

        [MenuItem("Tools/Quad from sprite generator")]
        public static void ShowWindow()
        {
            QuadFromSpriteGenerator window = GetWindow<QuadFromSpriteGenerator>();
            window.titleContent = new GUIContent("Quad from sprite generator");
            window.position = new Rect(100, 100, 400, 75);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var objectField = new ObjectField("Sprite")
            {
                objectType = typeof(Sprite),
                value = _sprite
            };
            objectField.value = _sprite;
            objectField.RegisterValueChangedCallback(OnObjectFieldChanged);
            rootVisualElement.Add(objectField);

            Button button = new Button()
            {
                name = "Generate new quad",
                text = "Generate"
            };
            button.clicked += OnButtonClick;
            root.Add(button);
        }

        #region Listeners
        private void OnObjectFieldChanged(ChangeEvent<Object> callback)
        {
            _sprite = callback.newValue as Sprite;
            Debug.Log("Selected: " + _sprite.name);
        }

        private void OnButtonClick()
        {
            if (_sprite == null)
            {
                Debug.LogError("You don't have any sprite references to generate your quad.");
                return;
            }
            CreateQuadBasedOnSpriteRatio(_sprite);
        }
        #endregion

        private void CreateQuadBasedOnSpriteRatio(Sprite sprite)
        {
            var current = GameObject.CreatePrimitive(PrimitiveType.Quad);

            Vector2 spriteSize = sprite.bounds.size;
            Vector2 ratio = spriteSize / (spriteSize.x < spriteSize.y ? spriteSize.x : spriteSize.y);

            current.transform.position = new Vector3(0, 0, 0);
            current.transform.localScale = new Vector3(ratio.x, ratio.y, 1);

            current.name = _sprite.name;
        }
    }
}