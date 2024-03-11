using System;
using Frog.Level.Collections;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.LevelEditor.Data;
using Frog.LevelEditor.View;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.Tools
{
    internal sealed class DrawingTool : LevelEditorTool
    {
        private readonly CellSpritesProvider _cellSpritesProvider;

        private readonly BoardGridView _boardView;
        private readonly VisualElement _panelRoot;

        private readonly DrawingBrushesView _panel;

        private BoardPoint? _currentDrawPoint;

        private EventCallback<MouseDownEvent> _dnEvent;
        private EventCallback<MouseMoveEvent> _mvEvent;
        private EventCallback<MouseUpEvent> _upEvent;
        private EventCallback<MouseLeaveEvent> _lvEvent;


        public DrawingTool(CellSpritesProvider csp, BoardGridView boardView, VisualElement panelRoot)
        {
            _cellSpritesProvider = csp;
            _panel = new DrawingBrushesView(csp);
            _boardView = boardView;
            _panelRoot = panelRoot;
        }

        public override string Name => "Drawing";

        public override void Enable(LevelData levelData)
        {
            _panelRoot.Add(_panel);

            _dnEvent = e => StartDrawing(levelData, e);
            _mvEvent = e => UpdateDrawing(levelData, e);
            _upEvent = _ => EndDrawing(levelData);
            _lvEvent = _ => EndDrawing(levelData);

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

        private void StartDrawing(LevelData levelData, MouseDownEvent e)
        {
            Debug.Assert(_currentDrawPoint == null);
            var point = GetPoint(levelData, e.localMousePosition);

            _currentDrawPoint = point;
            UpdateCell(levelData, point);
        }

        private void UpdateDrawing(LevelData levelData, MouseMoveEvent e)
        {
            if ((e.pressedButtons & 1) == 0)
                _currentDrawPoint = null;

            if (_currentDrawPoint == null)
                return;

            var point = GetPoint(levelData, e.localMousePosition);
            if (_currentDrawPoint == point)
                return;

            _currentDrawPoint = point;
            UpdateCell(levelData, point);
        }

        private void EndDrawing(LevelData levelData)
        {
            _currentDrawPoint = null;
            levelData.SaveAsset();
        }

        private static BoardPoint GetPoint(LevelData levelData, in Vector2 point)
        {
            var x = (int)(point.x / 64);
            var y = levelData.Height - 1 - (int)(point.y / 64);
            return new BoardPoint(x, y);
        }

        private void UpdateCell(LevelData levelData, in BoardPoint point)
        {
            ref var cell = ref levelData.AsBoardGrid().RefMutAt(point);

            switch (_panel.Brush.Layer)
            {
                case DrawingLayer.Tiles:
                    cell.TileType = _panel.Brush.TileType;
                    cell.TileColor = _panel.Brush.Color;
                    break;
                case DrawingLayer.Objects:
                    cell.ObjectType = _panel.Brush.ObjectType;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var cellSprites = _cellSpritesProvider.GetSprites(cell);
            _boardView.SetSprites(point, cellSprites);
        }
    }
}