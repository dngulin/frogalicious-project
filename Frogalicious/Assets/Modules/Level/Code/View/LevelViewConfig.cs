using UnityEngine;

namespace Frog.Level.View
{
    [CreateAssetMenu(menuName = "Level View Config", fileName = nameof(LevelViewConfig))]
    public class LevelViewConfig : ScriptableObject
    {
        public GameObject Tile;
        public GameObject Obstacle;
        public GameObject Character;
    }
}