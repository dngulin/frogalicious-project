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

            for (var i = 0; i < 9; i++)
            {
                var row = new BoardRowView();
                _rows.Add(row);
                Add(row);
            }

            style.width = 64 * 9;
            style.height = 64 * 9;
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