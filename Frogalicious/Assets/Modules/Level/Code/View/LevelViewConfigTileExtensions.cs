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
                    var buttonPrefab = tile.State.AsButton.Color switch {
                        BoardColorGroup.Blue => config.ButtonBlue,
                        BoardColorGroup.Red => config.ButtonRed,
                        BoardColorGroup.Yellow => config.ButtonYellow,
                        BoardColorGroup.Green => config.ButtonGreen,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    var button = Object.Instantiate(buttonPrefab, position, Quaternion.identity, parent);
                    button.Init(tile.State.AsButton);
                    return button;
                }

                case BoardTileType.Spikes:
                {
                    var spikesPrefab = tile.State.AsButton.Color switch {
                        BoardColorGroup.Blue => config.SpikesBlue,
                        BoardColorGroup.Red => config.SpikesRed,
                        BoardColorGroup.Yellow => config.SpikesYellow,
                        BoardColorGroup.Green => config.SpikesGreen,
                        _ => throw new ArgumentOutOfRangeException()
                    };
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