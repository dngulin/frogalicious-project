using Frog.Level.Primitives;
using Frog.LevelEditor.Data;
using Frog.LevelEditor.View;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.Tools
{
    internal sealed class LevelSettingsTool : LevelEditorTool
    {
        private readonly BoardGridView _boardView;
        private readonly VisualElement _panelRoot;

        private readonly LevelSettingsView _settingsView = new LevelSettingsView();

        public LevelSettingsTool(BoardGridView boardView, VisualElement panelRoot)
        {
            _boardView = boardView;
            _panelRoot = panelRoot;
        }

        public override string Name => "Level Settings";

        public override void Enable(LevelData level)
        {
            _panelRoot.Add(_settingsView);

            var rows = level.Rows.Count;
            var cols = level.Rows.Count > 0 ? level.Rows[0].Count : 0;

            _settingsView.Height = rows;
            _settingsView.Width = cols;

            if (_settingsView.Width != cols || _settingsView.Height != rows)
                HandleBoardSizeChanged(level, _settingsView.Width, _settingsView.Height);

            _settingsView.OnSizeUpdated += (w, h) => HandleBoardSizeChanged(level, w, h);
        }

        public override void Disable()
        {
            _settingsView.ClearSubscriptions();
            _panelRoot.Remove(_settingsView);
        }

        private void HandleBoardSizeChanged(LevelData level, int w, int h)
        {
            if (level.ChangeBoardSize(w, h))
                ResizeAndRedrawBoard(w, h);
        }

        private void ResizeAndRedrawBoard(int w, int h)
        {
            _boardView.ChangeSize(w, h);

            for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
            {
                var point = new BoardPoint(x, y);
                _boardView.SetSprites(point, default);
            }
        }
    }
}