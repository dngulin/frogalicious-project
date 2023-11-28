using Frog.Level.Data;
using Frog.LevelEditor.Data;
using Frog.LevelEditor.View;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.Tools
{
    internal sealed class LevelSettingsTool : LevelEditorTool
    {
        private readonly CellSpritesProvider _cellSpritesProvider;

        private readonly BoardGridView _boardView;
        private readonly VisualElement _panelRoot;

        private readonly LevelSettingsView _settingsView = new LevelSettingsView();


        public LevelSettingsTool(CellSpritesProvider csp, BoardGridView boardView, VisualElement panelRoot)
        {
            _cellSpritesProvider = csp;

            _boardView = boardView;
            _panelRoot = panelRoot;
        }

        public override string Name => "Level Settings";

        public override void Enable(LevelData levelData)
        {
            _panelRoot.Add(_settingsView);

            _settingsView.Width = levelData.Width;
            _settingsView.Height = levelData.Height;

            if (_settingsView.Width != levelData.Width || _settingsView.Height != levelData.Height)
                HandleBoardSizeChanged(levelData, _settingsView.Width, _settingsView.Height);

            _settingsView.OnSizeUpdated += (w, h) => HandleBoardSizeChanged(levelData, w, h);
        }

        public override void Disable()
        {
            _settingsView.ClearSubscriptions();
            _panelRoot.Remove(_settingsView);
        }

        private void HandleBoardSizeChanged(LevelData levelData, int w, int h)
        {
            if (!levelData.ChangeBoardSize((ushort)w, (ushort)h))
                return;

            levelData.SaveAsset();
            _boardView.Reset(levelData, _cellSpritesProvider);
        }
    }
}