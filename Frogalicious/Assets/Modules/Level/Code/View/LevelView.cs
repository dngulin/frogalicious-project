using System;
using System.Collections.Generic;
using Frog.Level.Collections;
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

            var grid = data.AsBoardGrid();

            for (var y = 0; y < grid.Height; y++)
            for (var x = 0; x < grid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref grid.RefAt(point);

                if (cellData.TileType != BoardTileType.Nothing)
                {
                    var tilePrefab = cellData.TileType switch
                    {
                        BoardTileType.Nothing => throw new InvalidOperationException(),
                        BoardTileType.Ground => viewConfig.Ground,
                        BoardTileType.Button => viewConfig.Button,
                        BoardTileType.Spikes => viewConfig.Spikes,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    InstantiateAt(tilePrefab, point);
                }

                if (cellData.ObjectType != BoardObjectType.Nothing)
                {
                    var objPrefab = cellData.ObjectType switch
                    {
                        BoardObjectType.Nothing => throw new InvalidOperationException(),
                        BoardObjectType.Character => viewConfig.Character,
                        BoardObjectType.Obstacle => viewConfig.Obstacle,
                        BoardObjectType.Box => viewConfig.Box,
                        BoardObjectType.Coin => viewConfig.Coin,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    var obj = InstantiateAt(objPrefab, point);

                    if (cellData.ObjectType == BoardObjectType.Character)
                    {
                        _character = obj.transform;
                    }
                }
            }

            Debug.Assert(_character != null);

            var center = new Vector2(data.Width - 1, data.Height - 1) * 0.5f;
            camera.transform.position = new Vector3(center.x, center.y, -10);

            _camera = camera;
            UpdateOrthographicSize();
        }

        private GameObject InstantiateAt(GameObject prefab, in BoardPoint pos)
        {
            var instance = Object.Instantiate(prefab, _root);
            instance.transform.localPosition = pos.ToVector2();
            return instance;
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