using System;
using System.Collections.Generic;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.Level.Simulation;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Level.View
{
    public class LevelView : IDisposable
    {
        private readonly Transform _root;
        private readonly Transform _character;

        private readonly Camera _camera;

        public LevelView(LevelViewConfig viewConfig, LevelData data, Camera camera)
        {
            var rootGo = new GameObject("LevelRoot");
            _root = rootGo.transform;

            for (var y = 0; y < data.Height; y++)
            for (var x = 0; x < data.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref data.CellAtPoint(point);
                if (cellData.TileType == BoardTileType.Ground)
                {
                    var cell = Object.Instantiate(viewConfig.Tile, _root);
                    cell.transform.localPosition = point.ToVector2();
                }

                switch (cellData.ObjectType)
                {
                    case BoardObjectType.Character:
                        var hero = Object.Instantiate(viewConfig.Character, _root);
                        hero.transform.localPosition = point.ToVector2();
                        _character = hero.transform;
                        break;

                    case BoardObjectType.Obstacle:
                        var obstacle = Object.Instantiate(viewConfig.Obstacle, _root);
                        obstacle.transform.localPosition = point.ToVector2();
                        break;
                }
            }

            var center = new Vector2(data.Width - 1, data.Height - 1) * 0.5f;
            camera.transform.position = new Vector3(center.x, center.y, -10);

            _camera = camera;
            UpdateOrthographicSize();
        }

        public void Dispose()
        {
            if (_root != null)
                Object.Destroy(_root.gameObject);
        }

        private const float MoveDuration = 0.5f;
        private bool _moving;
        private float _moveTime;
        private Vector2 _moveStart;
        private Vector2 _moveEnd;

        public void Tick(float dt)
        {
            UpdateOrthographicSize();

            if (!_moving)
                return;

            _moveTime += dt;
            _character.localPosition = Vector2.Lerp(_moveStart, _moveEnd, _moveTime / MoveDuration);

            if (_moveTime >= MoveDuration)
                _moving = false;
        }

        public bool IsPlayingTimeline => _moving;

        public void StartPlayingTimeline(List<TimeLineEvent> timeLine)
        {
            foreach (var timeLineEvent in timeLine)
            {
                if (timeLineEvent.Type == TimeLineEventType.Move)
                {
                    _moving = true;
                    _moveTime = 0;
                    _moveStart = timeLineEvent.Position.ToVector2();
                    _moveEnd = timeLineEvent.EndPosition.ToVector2();
                }
            }
        }

        private const float MaxHeight = 7f;
        private const float MaxWidth = 11f;
        private float _lastAspect;

        private void UpdateOrthographicSize()
        {
            if (Mathf.Approximately(_lastAspect, _camera.aspect))
                return;

            _lastAspect = _camera.aspect;

            if (_lastAspect > 1)
            {
                _camera.orthographicSize = MaxHeight / 2;
            }
            else
            {
                _camera.orthographicSize = MaxWidth / (2 * _camera.aspect);
            }
        }
    }
}