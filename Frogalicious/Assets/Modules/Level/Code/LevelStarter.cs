using Frog.Level.Data;
using Frog.Level.View;
using UnityEngine;

namespace Frog.Level
{
    public class LevelStarter : MonoBehaviour
    {
        [SerializeField] private LevelData _data;
        [SerializeField] private LevelViewConfig _levelViewConfig;

        [SerializeField] private Camera _camera;

        private LevelController _level;

        private void Start()
        {
            _level = LevelController.Create(_data, _levelViewConfig, _camera);
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