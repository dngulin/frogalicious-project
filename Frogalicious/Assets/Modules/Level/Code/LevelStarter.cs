using Frog.Level.Data;
using Frog.Level.View;
using UnityEngine;

namespace Frog.Level
{
    public class LevelStarter : MonoBehaviour
    {
        [SerializeField]
        private LevelData _data;

        [SerializeField]
        private LevelViewConfig _levelViewConfig;

        private LevelView _view;

        private void Start()
        {
            _view = new LevelView(_levelViewConfig, _data);
        }
    }
}