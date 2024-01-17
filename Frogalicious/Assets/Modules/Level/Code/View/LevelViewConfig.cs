using System;
using UnityEngine;

namespace Frog.Level.View
{
    [CreateAssetMenu(menuName = "Level View Config", fileName = nameof(LevelViewConfig))]
    public class LevelViewConfig : ScriptableObject
    {
        public LevelViewPrefabs Prefabs;
        public LevelViewSprites Sprites;
    }

    [Serializable]
    public struct LevelViewPrefabs
    {
        public GroundTileView Ground;
        public ButtonTileView Button;
        public SpikesTileView Spikes;

        public StaticEntityView Character;
        public StaticEntityView Obstacle;
        public StaticEntityView Box;
        public StaticEntityView Coin;
    }

    [Serializable]
    public struct LevelViewSprites
    {
        public Sprite[] Ground;
        public ButtonSprites Button;
        public SpikesSprites Spikes;

        public Sprite Character;
        public Sprite Obstacle;
        public Sprite Box;
        public Sprite Coin;
    }

    [Serializable]
    public struct ButtonSprites
    {
        public Sprite Normal;
        public Sprite Pressed;
    }

    [Serializable]
    public struct SpikesSprites
    {
        public Sprite Active;
        public Sprite Inactive;
    }
}