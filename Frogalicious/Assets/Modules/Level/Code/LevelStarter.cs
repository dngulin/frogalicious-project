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

        private LevelController _level;

        private void Start()
        {
            _level = LevelController.Create(_data, _levelViewConfig);
        }

        private void Update()
        {
            _level.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _level.Dispose();
        }
    }
}