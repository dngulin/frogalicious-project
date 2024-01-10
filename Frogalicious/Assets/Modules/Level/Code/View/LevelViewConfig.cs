using UnityEngine;
using UnityEngine.Serialization;

namespace Frog.Level.View
{
    [CreateAssetMenu(menuName = "Level View Config", fileName = nameof(LevelViewConfig))]
    public class LevelViewConfig : ScriptableObject
    {
        public GameObject Ground;
        public GameObject Button;
        public GameObject Spikes;

        public GameObject Character;
        public GameObject Obstacle;
        public GameObject Box;
        public GameObject Coin;
    }
}