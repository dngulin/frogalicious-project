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
                Tile = _tiles[cellData.TileType],
                Object = _objects[cellData.ObjectType],
            };
        }
    }
}