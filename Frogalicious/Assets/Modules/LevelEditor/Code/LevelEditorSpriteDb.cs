using System.Collections.Generic;
using System.Linq;
using Frog.Level.Primitives;
using Frog.LevelEditor.Config;
using UnityEngine;

namespace Frog.LevelEditor
{
    public class LevelEditorSpriteDb
    {
        public readonly Dictionary<BoardTileType, Sprite> Tiles;
        public readonly Dictionary<BoardObjectType, Sprite> Objects;

        public LevelEditorSpriteDb(LevelEditorIcons icons)
        {
            Tiles = icons.Tiles.ToDictionary(e => e.Type, e => e.Sprite);
            Objects = icons.Objects.ToDictionary(e => e.Type, e => e.Sprite);;
        }
    }
}