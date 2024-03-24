using System;
using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.View
{
    [CreateAssetMenu(menuName = "Level View Config", fileName = nameof(LevelViewConfig))]
    public class LevelViewConfig : ScriptableObject
    {
        [Header("Tiles")]
        public GroundTileView Ground;
        public ColorVariants<ButtonTileView> ButtonVariants;
        public ColorVariants<SpikesTileView> SpikesVariants;
        public SpringTileView Spring;

        [Header("Objects")]
        public StaticEntityView Character;
        public StaticEntityView Obstacle;
        public StaticEntityView Box;
        public StaticEntityView Coin;
    }

    [Serializable]
    public struct ColorVariants<T>
    {
        [SerializeField] private T _blue;
        [SerializeField] private T _red;
        [SerializeField] private T _yellow;
        [SerializeField] private T _green;

        public readonly T this[BoardColorGroup color]
        {
            get
            {
                return color switch
                {
                    BoardColorGroup.Blue => _blue,
                    BoardColorGroup.Red => _red,
                    BoardColorGroup.Yellow => _yellow,
                    BoardColorGroup.Green => _green,
                    _ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
                };
            }
        }
    }
}