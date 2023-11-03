using System.Collections.Generic;
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

        private LevelData _level = new LevelData();

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

            SetupTools();
        }

        private void SetupTools()
        {
            var csp = new CellSpritesProvider(_icons);

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
            tool.Enable(_level);

            _currentToolType = toolType;
            _toolMenu.text = tool.Name;
        }
    }
}