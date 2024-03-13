using System.Collections.Generic;
using System.Linq;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.LevelEditor.Config;
using Frog.LevelEditor.View;
using UnityEngine;

namespace Frog.LevelEditor
{
    internal class CellSpritesProvider
    {
        private readonly Dictionary<BoardTileType, Sprite> _tiles;
        private readonly Dictionary<BoardObjectType, Sprite> _objects;

        public CellSpritesProvider(LevelEditorIcons icons)
        {
            _tiles = icons.Tiles.ToDictionary(e => e.Type, e => e.Sprite);
            _objects = icons.Objects.ToDictionary(e => e.Type, e => e.Sprite);
        }

        public CellSprites GetSprites(in CellData cellData)
        {
            return new CellSprites
            {
                Tile = _tiles[cellData.Tile.Type],
                TileTintColor = GetTintColor(cellData.Tile.Type, cellData.Tile.Color),
                TileRotation = GetRotation(cellData.Tile.Direction),
                Object = _objects[cellData.Object.Type],
            };
        }

        public Sprite GetTileSprite(BoardTileType type) => _tiles[type];
        public Sprite GetObjectSprite(BoardObjectType type) => _objects[type];

        private static Color GetTintColor(BoardTileType tileType, BoardColorGroup color)
        {
            switch (tileType)
            {
                case BoardTileType.Button:
                case BoardTileType.Spikes:
                    return GetTintColor(color);
                default:
                    return Color.white;
            }
        }

        public static Color GetTintColor(BoardColorGroup color)
        {
            return color switch
            {
                BoardColorGroup.Blue => Color.cyan,
                BoardColorGroup.Red => Color.red,
                BoardColorGroup.Yellow => Color.yellow,
                BoardColorGroup.Green => Color.green,
                _ => Color.black,
            };
        }

        public static Quaternion GetRotation(BoardDirection direction)
        {
            return direction switch
            {
                BoardDirection.Up => Quaternion.AngleAxis(0, Vector3.forward),
                BoardDirection.Right => Quaternion.AngleAxis(90, Vector3.forward),
                BoardDirection.Down => Quaternion.AngleAxis(180, Vector3.forward),
                BoardDirection.Left => Quaternion.AngleAxis(270, Vector3.forward),
                _ => Quaternion.identity,
            };
        }
    }
}