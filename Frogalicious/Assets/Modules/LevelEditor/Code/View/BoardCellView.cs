using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class BoardCellView : VisualElement
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
            _tile.tintColor = sprites.TileTintColor;
            _tile.transform.rotation = sprites.TileRotation;
            _object.sprite = sprites.Object;
        }
    }

    internal struct CellSprites
    {
        public Sprite Tile;
        public Color TileTintColor;
        public Quaternion TileRotation;
        public Sprite Object;
    }
}