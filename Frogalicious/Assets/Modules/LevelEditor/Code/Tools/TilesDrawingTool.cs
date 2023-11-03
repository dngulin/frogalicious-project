using System;
using Frog.Level.Primitives;
using Frog.LevelEditor.Data;
using Frog.LevelEditor.Tools;
using Frog.LevelEditor.View;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.Modules.LevelEditor.Code.Tools
{
    internal sealed class TilesDrawingTool : LevelEditorTool
    {
        private readonly CellSpritesProvider _cellSpritesProvider;

        private readonly BoardGridView _boardView;
        private readonly VisualElement _panelRoot;

        private readonly DrawingSettingsView _panel = new DrawingSettingsView();

        private BoardPoint? _currentDrawPoint;

        private EventCallback<MouseDownEvent> _dnEvent;
        private EventCallback<MouseMoveEvent> _mvEvent;
        private EventCallback<MouseUpEvent> _upEvent;
        private EventCallback<MouseLeaveEvent> _lvEvent;


        public TilesDrawingTool(CellSpritesProvider csp, BoardGridView boardView, VisualElement panelRoot)
        {
            _cellSpritesProvider = csp;
            _boardView = boardView;
            _panelRoot = panelRoot;
        }

        public override string Name => "Draw Tiles";

        public override void Enable(LevelData level)
        {
            _panelRoot.Add(_panel);

            _dnEvent = e => StartDrawing(level, e);
            _mvEvent = e => UpdateDrawing(level, e);
            _upEvent = _ => EndDrawing();
            _lvEvent = _ => EndDrawing();

            _boardView.RegisterCallback(_dnEvent);
            _boardView.RegisterCallback(_mvEvent);
            _boardView.RegisterCallback(_upEvent);
            _boardView.RegisterCallback(_lvEvent);
        }

        public override void Disable()
        {
            Debug.Assert(_currentDrawPoint == null);

            _boardView.UnregisterCallback(_dnEvent);
            _boardView.UnregisterCallback(_mvEvent);
            _boardView.UnregisterCallback(_upEvent);
            _boardView.UnregisterCallback(_lvEvent);

            _dnEvent = null;
            _mvEvent = null;
            _upEvent = null;
            _lvEvent = null;

            _panelRoot.Remove(_panel);
        }

        private void StartDrawing(LevelData level, MouseDownEvent e)
        {
            Debug.Assert(_currentDrawPoint == null);
            var point = GetPoint(level, e.localMousePosition);

            _currentDrawPoint = point;
            UpdateCell(level, point);
        }

        private void UpdateDrawing(LevelData level, MouseMoveEvent e)
        {
            if ((e.pressedButtons & 1) == 0)
                _currentDrawPoint = null;

            if (_currentDrawPoint == null)
                return;

            var point = GetPoint(level, e.localMousePosition);
            if (_currentDrawPoint == point)
                return;

            _currentDrawPoint = point;
            UpdateCell(level, point);
        }

        private void EndDrawing() => _currentDrawPoint = null;

        private static BoardPoint GetPoint(LevelData level, in Vector2 point)
        {
            var x = (int)(point.x / 64);
            var y = level.Rows.Count - 1 - (int)(point.y / 64);
            return new BoardPoint(x, y);
        }

        private void UpdateCell(LevelData level, in BoardPoint point)
        {
            var cell = level.Rows[point.Y][point.X];

            switch (_panel.Layer)
            {
                case DrawingLayer.Tiles:
                    cell.TileType = _panel.TileType;
                    break;
                case DrawingLayer.Objects:
                    cell.ObjectType = _panel.ObjectType;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            level.Rows[point.Y][point.X] = cell;

            var cellSprites = _cellSpritesProvider.GetSprites(cell);
            _boardView.SetSprites(point, cellSprites);
        }
    }
}