using System;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public static class LevelViewConfigTileExtensions
    {
        public static EntityView CreateTile(this LevelViewConfig config, in TileState tile, Transform parent, Vector2 position)
        {
            switch (tile.Type)
            {
                case BoardTileType.Nothing:
                    throw new InvalidOperationException();

                case BoardTileType.Ground:
                    return config.Ground.Spawn(parent, position).Initialized();

                case BoardTileType.Button:
                    var button = tile.State.AsButton;
                    return config.ButtonVariants[button.Color].Spawn(parent, position).Initialized(button);

                case BoardTileType.Spikes:
                    var spikes = tile.State.AsSpikes;
                    return config.SpikesVariants[spikes.Color].Spawn(parent, position).Initialized(spikes);

                case BoardTileType.Spring:
                    return config.Spring.Spawn(parent, position).Inintialized(tile.State.AsSpring);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}