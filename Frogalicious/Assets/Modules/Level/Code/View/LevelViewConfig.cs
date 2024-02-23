using UnityEngine;

namespace Frog.Level.View
{
    [CreateAssetMenu(menuName = "Level View Config", fileName = nameof(LevelViewConfig))]
    public class LevelViewConfig : ScriptableObject
    {
        [Header("Tiles")]
        public GroundTileView Ground;

        public ButtonTileView ButtonBlue;
        public ButtonTileView ButtonRed;

        public SpikesTileView SpikesBlue;
        public SpikesTileView SpikesRed;

        [Header("Objects")]
        public StaticEntityView Character;
        public StaticEntityView Obstacle;
        public StaticEntityView Box;
        public StaticEntityView Coin;
    }
}