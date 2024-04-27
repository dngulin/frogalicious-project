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
                    return config.ButtonVariants[tile.CfgHandle.AsColor].Spawn(parent, position).Initialized(button);

                case BoardTileType.Spikes:
                    var spikes = tile.State.AsSpikes;
                    return config.SpikesVariants[tile.CfgHandle.AsColor].Spawn(parent, position).Initialized(spikes);

                case BoardTileType.Spring:
                    return config.Spring.Spawn(parent, position).Initialized(tile.CfgHandle.AsDirection);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}