using System.Collections.Generic;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.LevelEditor.Config;
using Frog.LevelEditor.View;
using Frog.LevelEditor.Data;
using Frog.LevelEditor.Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow() => GetWindow<LevelEditorWindow>();

        [SerializeField] private VisualTreeAsset _windowLayout;
        [SerializeField] private LevelEditorIcons _icons;

        private ToolbarButton _openButton;
        private ToolbarButton _saveButton;
        private ToolbarMenu _toolMenu;

        private VisualElement _sidePanel;
        private BoardGridView _boardGridView;

        private LevelEditorToolType _currentToolType;

        private readonly Dictionary<LevelEditorToolType, LevelEditorTool> _tools =
            new Dictionary<LevelEditorToolType, LevelEditorTool>();

        private LevelData _levelData;
        private EditorLevelData _editorLevelData;

        public void CreateGUI()
        {
            titleContent = new GUIContent("Level");

            var root = rootVisualElement;
            _windowLayout.CloneTree(root);

            _openButton = root.Q<ToolbarButton>("OpenButton");
            _saveButton = root.Q<ToolbarButton>("SaveButton");
            _toolMenu = root.Q<ToolbarMenu>("ToolMenu");
            _sidePanel = root.Q<VisualElement>("SidePanel");

            _boardGridView = new BoardGridView();
            root.Q<ScrollView>("BoardScroll").Add(_boardGridView);

            var csp = new CellSpritesProvider(_icons);

            SetupLevelData(csp);
            SetupTools(csp);
        }

        private void SetupLevelData(CellSpritesProvider csp)
        {
            var guilds = AssetDatabase.FindAssets($"t:{typeof(LevelData)}");
            var assetPath = AssetDatabase.GUIDToAssetPath(guilds[0]);
            _levelData = AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
            _editorLevelData = EditorLevelDataConverter.CreateFrom(_levelData);

            _saveButton.clicked += () =>
            {
                _editorLevelData.WriteTo(_levelData);
                EditorUtility.SetDirty(_levelData);
                AssetDatabase.SaveAssetIfDirty(_levelData);
            };

            _boardGridView.ChangeSize(_levelData.Width, _levelData.Height);

            for (var y = 0; y < _levelData.Height; y++)
            for (var x = 0; x < _levelData.Width; x++)
            {
                var point = new BoardPoint(x, y);
                var cell = _editorLevelData.Rows[point.Y][point.X];
                var cellSprites = csp.GetSprites(cell);
                _boardGridView.SetSprites(point, cellSprites);
            }
        }

        private void SetupTools(CellSpritesProvider csp)
        {
            RegisterTool(LevelEditorToolType.Settings, new LevelSettingsTool(csp, _boardGridView, _sidePanel));
            RegisterTool(LevelEditorToolType.DrawTiles, new TilesDrawingTool(csp, _boardGridView, _sidePanel));

            SetCurrentTool(LevelEditorToolType.Settings);
        }

        private void RegisterTool(LevelEditorToolType toolType, LevelEditorTool tool)
        {
            _toolMenu.menu.AppendAction(tool.Name, _ => ChangeCurrentTool(toolType));
            _tools.Add(toolType, tool);
        }

        private void ChangeCurrentTool(LevelEditorToolType toolType)
        {
            if (_currentToolType == toolType)
                return;

            _tools[_currentToolType].Disable();
            SetCurrentTool(toolType);
        }

        private void SetCurrentTool(LevelEditorToolType toolType)
        {
            var tool = _tools[toolType];
            tool.Enable(_editorLevelData);

            _currentToolType = toolType;
            _toolMenu.text = tool.Name;
        }
    }
}