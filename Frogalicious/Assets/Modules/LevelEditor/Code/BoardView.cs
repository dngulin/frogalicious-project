using System.Collections.Generic;
using Frog.Level.Primitives;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class BoardView : VisualElement
    {
        private readonly List<BoardRowView> _rows = new List<BoardRowView>();

        public BoardView()
        {
            AddToClassList("board");
        }

        public void SetSprites(in BoardPoint point, in CellSprites sprites)
        {
            if (point.Y >= 0 && point.Y < _rows.Count)
            {
                _rows[point.Y].SetSprites(point.X, sprites);
            }
        }

        public void ChangeSize(int w, int h)
        {
            ChangeHeight(h);
            ChangeWidth(w);

            style.width = 64 * w;
            style.height = 64 * h;
        }

        private void ChangeHeight(int h)
        {
            while (_rows.Count < h)
            {
                var rowView = new BoardRowView();
                _rows.Add(rowView);
                Add(rowView);
            }

            while (_rows.Count > h)
            {
                var idx = _rows.Count - 1;
                _rows.RemoveAt(idx);
                RemoveAt(idx);
            }
        }

        private void ChangeWidth(int w)
        {
            foreach (var rowView in _rows)
            {
                rowView.ChangeWidth(w);
            }
        }
    }

    public class BoardRowView : VisualElement
    {
        private readonly List<BoardCellView> _cells = new List<BoardCellView>();

        public BoardRowView()
        {
            AddToClassList("board-row");

            for (var i = 0; i < 9; i++)
            {
                var cell = new BoardCellView();
                _cells.Add(cell);
                Add(cell);
            }
        }

        public void SetSprites(int idx, in CellSprites sprites)
        {
            if (idx >= 0 && idx < _cells.Count)
            {
                _cells[idx].SetSprites(sprites);
            }
        }

        public void ChangeWidth(int w)
        {
            while (_cells.Count < w)
            {
                var cellView = new BoardCellView();
                _cells.Add(cellView);
                Add(cellView);
            }

            while (_cells.Count > w)
            {
                var idx = _cells.Count - 1;
                _cells.RemoveAt(idx);
                RemoveAt(idx);
            }
        }
    }

    public class BoardCellView : VisualElement
    {
        private readonly Image _tile = new Image();
        private readonly Image _object = new Image();

        public BoardCellView()
        {
            AddToClassList("board-cell");
            _tile.AddToClassList("board-cell-image");
            _object.AddToClassList("board-cell-image");

            Add(_tile);
            Add(_object);
        }

        public void SetSprites(in CellSprites sprites)
        {
            _tile.sprite = sprites.Tile;
            _object.sprite = sprites.Object;
        }
    }

    public struct CellSprites
    {
        public readonly Sprite Tile;
        public readonly Sprite Object;

        public CellSprites(Sprite tile, Sprite obj)
        {
            Tile = tile;
            Object = obj;
        }
    }
}