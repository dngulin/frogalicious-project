using System;
using Frog.Core;
using Frog.Meta.MainMenu.View;
using UnityEngine;

namespace Frog.Meta.MainMenu
{
    public class MainMenuView : IDisposable
    {
        private OrthographicCameraFitter _cameraFitter;
        private MapView _map;

        public MainMenuView(Camera camera, MapView mapPrefab)
        {
            _map = UnityEngine.Object.Instantiate(mapPrefab);
            _cameraFitter = new OrthographicCameraFitter(camera, new Vector2(15, 10));
        }

        public void Dispose() => _map.DestroyGameObject();

        public void UpdateCamera() => _cameraFitter.UpdateSizeIfAspectChanged();

        public int? PollLevelClick() => _map.PollLevelClick();

        public void SetupCamera()
        {
            _cameraFitter.SetPosition(Vector2.zero);
            _cameraFitter.UpdateSize();
        }
    }
}