using UnityEngine;

namespace Frog.Level.View
{
    [CreateAssetMenu(menuName = "Level View Config", fileName = nameof(LevelViewConfig))]
    public class LevelViewConfig : ScriptableObject
    {
        public GroundTileView Ground;
        public ButtonTileView Button;
        public SpikesTileView Spikes;

        public StaticEntityView Character;
        public StaticEntityView Obstacle;
        public StaticEntityView Box;
        public StaticEntityView Coin;
    }
}