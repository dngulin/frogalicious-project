using UnityEngine;

namespace Frog.Core
{
    public struct OrthographicCameraFitter
    {
        private readonly Camera _camera;
        private readonly Vector2 _bounds;

        private float _aspect;

        public OrthographicCameraFitter(Camera camera, Vector2 bounds)
        {
            _camera = camera;
            _bounds = bounds;
            _aspect = 0;

            UpdateSize();
        }

        public readonly void SetPosition(Vector2 pos)
        {
            var z = _camera.transform.position.z;
            _camera.transform.position = new Vector3(pos.x, pos.y, z);
        }

        public void UpdateSizeIfAspectChanged()
        {
            if (Mathf.Approximately(_aspect, _camera.aspect))
                return;

            UpdateSize();
        }

        public void UpdateSize()
        {
            _aspect = _camera.aspect;

            if (_aspect > 1)
            {
                _camera.orthographicSize = _bounds.y / 2;
            }
            else
            {
                _camera.orthographicSize = _bounds.x / (2 * _aspect);
            }
        }
    }
}