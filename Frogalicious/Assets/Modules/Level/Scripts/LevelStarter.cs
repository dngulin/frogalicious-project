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

        private LevelData _editorLevelData;

        public void SetEditorLevelData(LevelData levelData)
        {
            _editorLevelData = levelData;
        }

        private void Start()
        {
            var data = _editorLevelData == null ? _data : _editorLevelData;
            _editorLevelData = null;

            _level = LevelController.Create(data, _levelViewConfig, _camera);
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