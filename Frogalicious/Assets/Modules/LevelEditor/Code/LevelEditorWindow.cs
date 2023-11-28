using System.Collections.Generic;
using System.Linq;
using Frog.Level.Data;
using Frog.LevelEditor.Config;
using Frog.LevelEditor.Data;
using Frog.LevelEditor.View;
using Frog.LevelEditor.Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        private const string PathPrefix = "Assets/Levels/";
        private const string PathSuffix = ".asset";


        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow() => GetWindow<LevelEditorWindow>();

        [SerializeField] private VisualTreeAsset _windowLayout;
        [SerializeField] private LevelEditorIcons _icons;

        private CellSpritesProvider _cellSpritesProvider;

        private ToolbarMenu _levelsMenu;
        private ToolbarButton _saveButton;
        private ToolbarMenu _toolMenu;

        private VisualElement _sidePanel;
        private BoardGridView _boardGridView;

        private readonly Dictionary<LevelEditorToolType, LevelEditorTool> _tools =
            new Dictionary<LevelEditorToolType, LevelEditorTool>();

        private LevelEditorTool _currentTool;

        private string _levelDataPath;
        private LevelData _levelData;

        public void RebuildLevelList(Dictionary<string, string> renames)
        {
            var levelPaths = GetLevelAssetsPaths();

            _levelsMenu.menu.ClearItems();
            CreateLevelMenuEntries(levelPaths);

            if (renames.TryGetValue(_levelDataPath, out var newPath) &&
                newPath.StartsWith(PathPrefix) &&
                newPath.EndsWith(PathSuffix))
            {
                _levelDataPath = newPath;
                _levelsMenu.text = StripLevelPath(newPath);
                return;
            }

            if (levelPaths.Contains(_levelDataPath))
                return;

            SelectLevel(levelPaths[0]);
        }

        private void CreateGUI()
        {
            titleContent = new GUIContent("LevelEditor");

            _cellSpritesProvider = new CellSpritesProvider(_icons);

            var root = rootVisualElement;
            _windowLayout.CloneTree(root);

            _levelsMenu = root.Q<ToolbarMenu>("LevelsMenu");
            _saveButton = root.Q<ToolbarButton>("SaveButton");
            _toolMenu = root.Q<ToolbarMenu>("ToolMenu");
            _sidePanel = root.Q<VisualElement>("SidePanel");
            _sidePanel = root.Q<VisualElement>("SidePanel");

            _boardGridView = new BoardGridView();
            root.Q<ScrollView>("BoardScroll").Add(_boardGridView);

            CreateTools();

            var levelPaths = GetLevelAssetsPaths();

            CreateLevelMenuEntries(levelPaths);
            SelectLevel(levelPaths[0]);
        }

        private static string[] GetLevelAssetsPaths()
        {
            var assets = AssetDatabase.FindAssets($"t:{nameof(LevelData)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(s => s.StartsWith(PathPrefix) && s.EndsWith(PathSuffix))
                .ToArray();

            if (assets.Length > 0)
                return assets;

            var levelData = CreateInstance<LevelData>();
            const string path = "Assets/Levels/NewLevel.asset";

            AssetDatabase.CreateAsset(levelData, path);
            levelData.SaveAsset();

            return new[] { path };
        }

        private void CreateLevelMenuEntries(string[] levelsPaths)
        {
            foreach (var levelPath in levelsPaths)
                _levelsMenu.menu.AppendAction(StripLevelPath(levelPath), _ => SelectLevel(levelPath));
        }

        private void SelectLevel(string levelPath)
        {
            _levelsMenu.text = StripLevelPath(levelPath);

            LoadLevel(levelPath);
            _boardGridView.Reset(_levelData, _cellSpritesProvider);

            if (_currentTool == null)
            {
                SetCurrentTool(_tools[LevelEditorToolType.Settings]);
            }
            else
            {
                _currentTool.Disable();
                _currentTool.Enable(_levelData);
            }
        }

        private static string StripLevelPath(string path) => path[PathPrefix.Length..^PathSuffix.Length];

        private void LoadLevel(string levelPath)
        {
            _levelDataPath = levelPath;
            _levelData = AssetDatabase.LoadAssetAtPath<LevelData>(levelPath);
        }

        private void CreateTools()
        {
            CreateTool(LevelEditorToolType.Settings, new LevelSettingsTool(_cellSpritesProvider, _boardGridView, _sidePanel));
            CreateTool(LevelEditorToolType.DrawTiles, new TilesDrawingTool(_cellSpritesProvider, _boardGridView, _sidePanel));
        }

        private void CreateTool(LevelEditorToolType toolType, LevelEditorTool tool)
        {
            _toolMenu.menu.AppendAction(tool.Name, _ => ChangeCurrentTool(toolType));
            _tools.Add(toolType, tool);
        }

        private void ChangeCurrentTool(LevelEditorToolType toolType)
        {
            var tool = _tools[toolType];
            if (_currentTool == tool)
                return;

            _currentTool.Disable();
            SetCurrentTool(tool);
        }

        private void SetCurrentTool(LevelEditorTool tool)
        {
            _currentTool = tool;
            _toolMenu.text = tool.Name;

            _currentTool.Enable(_levelData);
        }
    }
}