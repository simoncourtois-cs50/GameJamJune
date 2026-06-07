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
                TryAutoComplete();
                RefreshPreview();
            });
            root.Add(_levelNameField);
            root.Add(MakeSpacer(12));

            // --- Scene List -----
            _scenesLabel = MakeLabel("Scenes");
            root.Add(_scenesLabel);

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
                    if (_activeSceneIndex >= _sceneNames.Count - 1)
                        _activeSceneIndex = _sceneNames.Count - 2;
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
            var createBtn = new Button(OnCreate) { text = "Create / Update" };
            createBtn.style.height = 36;
            createBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            createBtn.style.marginTop = 8;
            root.Add(createBtn);

            // --- Initial Preview Refresh -----
            rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnFirstLayout);
        }

        #endregion


        #region Public API

        [MenuItem("Tools/Level Factory")]
        public static void ShowWindow()
        {
            LevelFactory window = GetWindow<LevelFactory>();
            window.titleContent = new GUIContent("Level Factory");
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

            SceneReference activeSceneRef = null;

            for (int i = 0; i < createdSceneAssets.Count; i++)
            {
                var sceneRef = new SceneReference();
                sceneRef.m_sceneAsset = createdSceneAssets[i];
                sceneRef.Sync();
                levelData.m_scenes.Add(sceneRef);

                if (i == _activeSceneIndex)
                    activeSceneRef = sceneRef;
            }

            levelData.m_activeScene = activeSceneRef ?? levelData.m_scenes[0];

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

        private void TryAutoComplete()
        {
            string trimmedBase = _basePath.Trim('/').Trim('\\');
            string trimmedName = _levelName.Trim();

            if (string.IsNullOrEmpty(trimmedName)) return;

            string levelFolderPath = $"Assets/{trimmedBase}/{trimmedName}";
            string scenesFolderPath = $"{levelFolderPath}/Scenes";

            if (!AssetDatabase.IsValidFolder(scenesFolderPath))
            {
                UpdateScenesLabel(isExisting: false);
                return;
            }

            string levelDataPath = $"{levelFolderPath}/{trimmedName}.asset";
            LevelData existingLevelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelDataPath);

            string[] guids = AssetDatabase.FindAssets("t:SceneAsset", new[] { scenesFolderPath });

            if (guids.Length == 0) return;

            string prefix = $"{trimmedName}_";
            List<string> foundSceneNames = new List<string>();
            string activeScenePath = existingLevelData?.m_activeScene?.ScenePath;

            _activeSceneIndex = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);

                string shortName = fileName.StartsWith(prefix)
                    ? fileName.Substring(prefix.Length)
                    : fileName;

                foundSceneNames.Add(shortName);

                if (!string.IsNullOrEmpty(activeScenePath) && path == activeScenePath)
                    _activeSceneIndex = foundSceneNames.Count - 1;
            }

            _sceneNames = foundSceneNames;
            RefreshSceneList();
            UpdateScenesLabel(isExisting: true);
            RefreshPreview();
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

        private void UpdateScenesLabel(bool isExisting)
        {
            if (_scenesLabel == null) return;
            _scenesLabel.text = isExisting ? "Scenes  (loaded from existing level)" : "Scenes";
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

                if (i == _activeSceneIndex)
                    row.style.backgroundColor = new Color(0.45f, 0.2f, 0.2f, 0.4f);

                var radio = new Toggle();
                radio.value = (i == _activeSceneIndex);
                radio.style.width = 20;
                radio.style.marginRight = 4;
                radio.RegisterValueChangedCallback(e =>
                {
                    if (e.newValue)
                    {
                        _activeSceneIndex = index;
                        RefreshSceneList();
                        RefreshPreview();
                    }
                    else
                    {
                        radio.SetValueWithoutNotify(true);
                    }
                });

                var indexLabel = new Label(i == _activeSceneIndex ? "★" : $"{i + 1}.");
                indexLabel.style.width = 20;
                indexLabel.style.color = (i == _activeSceneIndex) ? new Color(1f, 0.2f, 0.2f, 1f) : Color.white;
                indexLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                if (i == _activeSceneIndex)
                    indexLabel.tooltip = "This scene will be set as the active scene (SetActiveScene)";

                var field = new TextField { value = _sceneNames[i] };
                field.style.flexGrow = 1;
                field.RegisterValueChangedCallback(e =>
                {
                    _sceneNames[index] = e.newValue;
                    RefreshPreview();
                });

                var removeBtn = new Button(() =>
                {
                    if (_sceneNames.Count > 1)
                    {
                        _sceneNames.RemoveAt(index);
                        if (_activeSceneIndex >= _sceneNames.Count)
                            _activeSceneIndex = _sceneNames.Count - 1;
                        RefreshSceneList();
                        RefreshPreview();
                    }
                })
                { text = "✕" };
                removeBtn.style.width = 24;
                removeBtn.style.marginLeft = 2;

                row.Add(radio);
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

            for (int i = 0; i < _sceneNames.Count; i++)
            {
                string suffix = (i == _activeSceneIndex) ? "  ← active" : "";
                sb.AppendLine($"    {trimmedName}_{_sceneNames[i]}.unity{suffix}");
            }

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
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(PREF_BASE_PATH, _basePath);
        }

        private static bool IsValidFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9_\-]+$");
        }

        #endregion


        #region Private and Protected

        private string _basePath = "_/Database/Levels";
        private string _levelName = "";
        private List<string> _sceneNames = new List<string> { "Gameplay" };
        private int _activeSceneIndex = 0;

        private ScrollView _sceneListScrollView;
        private TextField _basePathField;
        private TextField _levelNameField;
        private Label _previewLabel;
        private Label _scenesLabel;

        private const string PREF_BASE_PATH = "LevelFactory_BasePath";

        #endregion
    }
}