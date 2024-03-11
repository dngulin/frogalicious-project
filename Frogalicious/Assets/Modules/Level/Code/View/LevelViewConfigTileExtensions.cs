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
                {
                    var ground = config.Ground.Spawn(parent, position);
                    ground.Init();
                    return ground;
                }

                case BoardTileType.Button:
                {
                    var button = config.ButtonVariants[tile.State.AsButton.Color].Spawn(parent, position);
                    button.Init(tile.State.AsButton);
                    return button;
                }

                case BoardTileType.Spikes:
                {
                    var spikes = config.SpikesVariants[tile.State.AsSpikes.Color].Spawn(parent, position);
                    spikes.Init(tile.State.AsSpikes);
                    return spikes;
                }

                case BoardTileType.Spring:
                {
                    var spring = config.Spring.Spawn(parent, position);
                    spring.Inint(tile.State.AsSpring);
                    return spring;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}