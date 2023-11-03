using System;
using System.Collections.Generic;
using Frog.Level.Primitives;
using Frog.LevelEditor.View;
using Frog.LevelEditor.Data;
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

        private ToolbarButton _openButton;
        private ToolbarButton _saveButton;
        private ToolbarMenu _toolMenu;

        private VisualElement _sidePanel;
        private BoardGridView _boardGridView;

        private LevelEditorMode _mode;

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
            foreach (var mode in new[] { LevelEditorMode.Settings, LevelEditorMode.Draw })
            {
                _toolMenu.menu.AppendAction(mode.ToString(), _ => ChangeCurrentMode(mode));
            }

            SetCurrentMode(LevelEditorMode.Settings);
        }

        private void ChangeCurrentMode(LevelEditorMode mode)
        {
            if (_mode == mode)
            {
                return;
            }

            _sidePanel.Clear();
            SetCurrentMode(mode);
        }

        private void SetCurrentMode(LevelEditorMode mode)
        {
            _mode = mode;
            _toolMenu.text = mode.ToString();

            switch (_mode)
            {
                case LevelEditorMode.Settings:
                    var lsv = new LevelSettingsView();
                    BindSettingsPanelToData(lsv);
                    _sidePanel.Add(lsv);
                    break;
                case LevelEditorMode.Draw:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void BindSettingsPanelToData(LevelSettingsView lsv)
        {
            lsv.Height = _level.Rows.Count;
            lsv.Width = _level.Rows.Count > 0 ? _level.Rows[0].Count : 0;

            if (ChangeBoardSize(lsv.Width, lsv.Height))
            {
                ResizeBoardView(lsv.Width, lsv.Height);
            }

            lsv.OnSizeUpdated += (w, h) =>
            {
                if (ChangeBoardSize(w, h))
                {
                    // TODO: Add history entry
                    ResizeBoardView(w, h);
                }
            };
        }

        private bool ChangeBoardSize(int w, int h)
        {
            var hChanged = ChangeBoardHeight(h);
            var wChanged = ChangeBoardWidth(w);
            return hChanged || wChanged;
        }

        private bool ChangeBoardHeight(int h)
        {
            if (_level.Rows.Count < h)
            {
                while (_level.Rows.Count < h)
                    _level.Rows.Add(new List<CellData>());

                return true;
            }

            if (_level.Rows.Count > h)
            {
                while (_level.Rows.Count > h)
                    _level.Rows.RemoveAt(_level.Rows.Count - 1);

                return true;
            }

            return false;
        }

        private bool ChangeBoardWidth(int w)
        {
            var updated = false;

            foreach (var row in _level.Rows)
            {
                if (row.Count < w)
                {
                    while (row.Count < w) row.Add(default);
                    updated = true;
                }
                else if (row.Count > w)
                {
                    while (row.Count > w) row.RemoveAt(row.Count - 1);
                    updated = true;
                }
            }

            return updated;
        }

        private void ResizeBoardView(int w, int h)
        {
            _boardGridView.ChangeSize(w, h);

            for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
            {
                var point = new BoardPoint(x, y);
                _boardGridView.SetSprites(point, default);
            }
        }
    }
}