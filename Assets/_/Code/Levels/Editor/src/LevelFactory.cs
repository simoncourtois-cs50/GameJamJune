using Levels.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Levels.Editor
{
    public class LevelFactory : EditorWindow
    {
        #region Unity API

        public void CreateGUI()
        {
            LoadPrefs();

            VisualElement root = rootVisualElement;

            root.style.paddingTop = 12;
            root.style.paddingBottom = 12;
            root.style.paddingLeft = 12;
            root.style.paddingRight = 12;

            // --- Base Path -----
            root.Add(MakeLabel("Base Path"));
            _basePathField = new TextField { value = _basePath };
            _basePathField.RegisterValueChangedCallback(evt =>
            {
                _basePath = evt.newValue;
                SavePrefs();
                RefreshPreview();
            });

            root.Add(_basePathField);
            root.Add(MakeSpacer(8));

            // --- Level Name -----
            root.Add(MakeLabel("Level Name"));
            _levelNameField = new TextField { value = _levelName };
            _levelNameField.RegisterValueChangedCallback(evt =>
            {
                _levelName = evt.newValue;
                SavePrefs();
                RefreshPreview();
            });
            root.Add(_levelNameField);
            root.Add(MakeSpacer(12));

            // --- Scene List -----
            root.Add(MakeLabel("Scenes"));
            _sceneListScrollView = new ScrollView();
            _sceneListScrollView.style.maxHeight = 100;

            RefreshSceneList();

            root.Add(_sceneListScrollView);
            root.Add(MakeSpacer(4));

            var sceneButtons = new VisualElement();
            sceneButtons.style.flexDirection = FlexDirection.Row;

            var addBtn = new Button(() =>
            {
                _sceneNames.Add("NewScene");
                RefreshSceneList();
                RefreshPreview();
            })
            { text = "+ Add Scene" };
            addBtn.style.flexGrow = 1;

            var removeBtn = new Button(() =>
            {
                if (_sceneNames.Count > 1)
                {
                    _sceneNames.RemoveAt(_sceneNames.Count - 1);
                    RefreshSceneList();
                    RefreshPreview();
                }
            })
            { text = "- Remove Last" };
            removeBtn.style.flexGrow = 1;

            sceneButtons.Add(addBtn);
            sceneButtons.Add(removeBtn);

            root.Add(sceneButtons);
            root.Add(MakeSpacer(12));


            // --- Preview -----
            root.Add(MakeLabel("Preview"));
            var previewScroll = new ScrollView();
            previewScroll.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.4f);
            previewScroll.style.maxHeight = 100;

            _previewLabel = new Label();
            _previewLabel.style.paddingTop = 6;
            _previewLabel.style.paddingBottom = 6;
            _previewLabel.style.paddingLeft = 8;
            _previewLabel.style.paddingRight = 8;
            _previewLabel.style.whiteSpace = WhiteSpace.Normal;

            previewScroll.Add(_previewLabel);
            root.Add(previewScroll);

            // --- Create Button -----
            var createBtn = new Button(OnCreate) { text = "Create Level" };
            createBtn.style.height = 36;
            createBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            createBtn.style.marginTop = 8;
            root.Add(createBtn);

            // --- Initial Preview Refresh -----
            rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnFirstLayout);
        }

        #endregion


        #region Public API

        [MenuItem("Tools/Create Level")]
        public static void ShowWindow()
        {
            LevelFactory window = GetWindow<LevelFactory>();
            window.titleContent = new GUIContent("Create Level");
        }

        #endregion


        #region Main API

        private void OnCreate()
        {
            // --- Handle untitled scenes -----
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (string.IsNullOrEmpty(scene.path))
                {
                    EditorUtility.DisplayDialog(
                        "Level Factory",
                        "Please save or close all untitled scenes before creating a level.",
                        "OK"
                    );
                    return;
                }
            }

            // --- Validation -----
            string trimmedBase = _basePath.Trim('/').Trim('\\');
            string trimmedName = _levelName.Trim();

            if (string.IsNullOrEmpty(trimmedName))
            {
                EditorUtility.DisplayDialog("Level Factory", "Please enter a level name.", "OK");
                return;
            }

            if (!IsValidFileName(trimmedName))
            {
                EditorUtility.DisplayDialog("Level Factory", $"Level name '{trimmedName}' contains invalid characters.", "OK");
                return;
            }

            foreach (string sceneName in _sceneNames)
            {
                if (string.IsNullOrEmpty(sceneName))
                {
                    EditorUtility.DisplayDialog("Level Factory", "Please enter a scene name.", "OK");
                    return;
                }

                if (!IsValidFileName(sceneName))
                {
                    EditorUtility.DisplayDialog("Level Factory", $"Scene name '{sceneName}' contains invalid characters.", "OK");
                    return;
                }
            }

            var sceneNamesSet = new HashSet<string>();
            foreach (string sceneName in _sceneNames)
            {
                if (!sceneNamesSet.Add(sceneName))
                {
                    EditorUtility.DisplayDialog("Level Factory", $"Duplicate scene name '{sceneName}'. All scene names must be unique.", "OK");
                    return;
                }
            }

            if (_sceneNames.Count == 0)
            {
                EditorUtility.DisplayDialog("Level Factory", "Add at least one scene.", "OK");
                return;
            }

            // --- Paths -----
            string levelFolderPath = $"Assets/{trimmedBase}/{trimmedName}";
            string scenesFolderPath = $"{levelFolderPath}/Scenes";

            // --- Create folders -----
            CreateFolderRecursive(levelFolderPath);
            CreateFolderRecursive(scenesFolderPath);

            // --- Create scenes & collect SceneAssets -----
            List<SceneAsset> createdSceneAssets = new List<SceneAsset>();

            foreach (string sceneName in _sceneNames)
            {
                string sceneAssetPath = $"{scenesFolderPath}/{trimmedName}_{sceneName}.unity";

                if (!File.Exists(Path.Combine(Application.dataPath, "../", sceneAssetPath)))
                {
                    var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    EditorSceneManager.SaveScene(newScene, sceneAssetPath);
                    EditorSceneManager.CloseScene(newScene, true);
                }

                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneAssetPath);
                if (sceneAsset != null) createdSceneAssets.Add(sceneAsset);
            }

            // --- Add scenes to Build Settings -----
            AddScenesToBuildSettings(createdSceneAssets);

            // --- Create LevelData ScriptableObject -----
            string levelDataPath = $"{levelFolderPath}/{trimmedName}.asset";
            LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);

            if (levelData == null)
            {
                levelData = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(levelData, levelDataPath);
            }

            // --- Populate SceneReferences -----
            levelData.m_scenes.Clear();

            foreach (SceneAsset sceneAsset in createdSceneAssets)
            {
                var sceneRef = new SceneReference();
                sceneRef.m_sceneAsset = sceneAsset;
                sceneRef.Sync();
                levelData.m_scenes.Add(sceneRef);
            }

            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // --- Ping the created asset -----
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelData;
            EditorGUIUtility.PingObject(levelData);

            EditorUtility.DisplayDialog(
                "Level Factory",
                $"Level '{trimmedName}' created successfully!\n\n" +
                $"Path: {levelFolderPath}\n" +
                $"Scenes: {createdSceneAssets.Count} added to Build Settings.",
                "OK"
            );
        }

        private void OnFirstLayout(GeometryChangedEvent evt)
        {
            RefreshPreview();
            rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnFirstLayout);
        }

        private static void CreateFolderRecursive(string assetPath)
        {
            string[] parts = assetPath.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static void AddScenesToBuildSettings(List<SceneAsset> scenes)
        {
            var existingScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            foreach (SceneAsset scene in scenes)
            {
                string path = AssetDatabase.GetAssetPath(scene);
                bool alreadyPresent = existingScenes.Exists(s => s.path == path);
                if (!alreadyPresent)
                    existingScenes.Add(new EditorBuildSettingsScene(path, true));
            }

            EditorBuildSettings.scenes = existingScenes.ToArray();
        }

        private static Label MakeLabel(string text)
        {
            Label label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 2;
            return label;
        }

        private void RefreshSceneList()
        {
            _sceneListScrollView.Clear();

            for (int i = 0; i < _sceneNames.Count; i++)
            {
                int index = i;
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginBottom = 2;

                var indexLabel = new Label($"{i + 1}.");
                indexLabel.style.width = 24;
                indexLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

                var field = new TextField { value = _sceneNames[i] };
                field.style.flexGrow = 1;
                field.RegisterValueChangedCallback(e =>
                {
                    _sceneNames[index] = e.newValue;
                    SavePrefs();
                    RefreshPreview();
                });

                var removeBtn = new Button(() =>
                {
                    if (_sceneNames.Count > 1)
                    {
                        _sceneNames.RemoveAt(index);
                        RefreshSceneList();
                        SavePrefs();
                        RefreshPreview();
                    }
                })
                { text = "✕" };
                removeBtn.style.width = 24;
                removeBtn.style.marginLeft = 2;

                row.Add(indexLabel);
                row.Add(field);
                row.Add(removeBtn);
                _sceneListScrollView.Add(row);
            }
        }

        private void RefreshPreview()
        {
            if (_previewLabel == null) return;

            string trimmedBase = (_basePath ?? "").Trim('/').Trim('\\');
            string trimmedName = (_levelName ?? "").Trim();
            string root = $"Assets/{trimmedBase}/{trimmedName}/";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{root}{trimmedName}.asset");
            sb.AppendLine($"{root}Scenes/");
            foreach (var s in _sceneNames)
                sb.AppendLine($"    {trimmedName}_{s}.unity");

            _previewLabel.text = sb.ToString().TrimEnd();
        }

        private static VisualElement MakeSpacer(float height)
        {
            var spacer = new VisualElement();
            spacer.style.height = height;
            return spacer;
        }

        private void LoadPrefs()
        {
            _basePath = EditorPrefs.GetString(PREF_BASE_PATH, "_/Database/Levels");
            _levelName = EditorPrefs.GetString(PREF_LEVEL_NAME, "NewLevel");

            string raw = EditorPrefs.GetString(PREF_SCENE_NAMES, "Gameplay");
            _sceneNames = new List<string>(raw.Split(';'));
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(PREF_BASE_PATH, _basePath);
            EditorPrefs.SetString(PREF_LEVEL_NAME, _levelName);
            EditorPrefs.SetString(PREF_SCENE_NAMES, string.Join(";", _sceneNames));
        }

        private static bool IsValidFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9_\-]+$");
        }

        #endregion


        #region Private and Protected

        private string _basePath = "_/Database/Levels";
        private string _levelName = "NewLevel";
        private List<string> _sceneNames = new List<string> { "Gameplay", "Environments"};

        private ScrollView _sceneListScrollView;
        private TextField _basePathField;
        private TextField _levelNameField;
        private Label _previewLabel;

        private const string PREF_BASE_PATH = "LevelFactory_BasePath";
        private const string PREF_LEVEL_NAME = "LevelFactory_LevelName";
        private const string PREF_SCENE_NAMES = "LevelFactory_SceneNames";

        #endregion
    }
}