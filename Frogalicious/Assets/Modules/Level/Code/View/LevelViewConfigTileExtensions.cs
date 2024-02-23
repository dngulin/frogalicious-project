using System;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;
using Object = UnityEngine.Object;

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
                    var ground = Object.Instantiate(config.Ground, position, Quaternion.identity, parent);
                    ground.Init();
                    return ground;
                }

                case BoardTileType.Button:
                {
                    var buttonPrefab = config.ButtonVariants[tile.State.AsButton.Color];
                    var button = Object.Instantiate(buttonPrefab, position, Quaternion.identity, parent);
                    button.Init(tile.State.AsButton);
                    return button;
                }

                case BoardTileType.Spikes:
                {
                    var spikesPrefab = config.SpikesVariants[tile.State.AsSpikes.Color];
                    var spikes = Object.Instantiate(spikesPrefab, position, Quaternion.identity, parent);
                    spikes.Init(tile.State.AsSpikes);
                    return spikes;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}