using System;
using System.Collections.Generic;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.Level.Simulation;
using Frog.Level.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Level.View
{
    public class LevelView : IDisposable
    {
        private readonly Transform _root;
        private readonly Dictionary<ushort, Transform> _objects = new Dictionary<ushort, Transform>();

        private readonly LevelViewConfig _viewConfig;
        private readonly Camera _camera;

        public LevelView(LevelViewConfig viewConfig, LevelData data, Camera camera)
        {
            var rootGo = new GameObject("LevelRoot");
            _root = rootGo.transform;

            var center = new Vector2(data.Width - 1, data.Height - 1) * 0.5f;
            camera.transform.position = new Vector3(center.x, center.y, -10);

            _viewConfig = viewConfig;
            _camera = camera;
            UpdateOrthographicSize();
        }

        public void CreateInitialObjects(in LevelState state)
        {
            for (var y = 0; y < state.Cells.Height; y++)
            for (var x = 0; x < state.Cells.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cell = ref state.Cells.RefReadonlyAt(point);

                if (cell.Tile.Type != BoardTileType.Nothing)
                {
                    var tilePrefab = cell.Tile.Type switch
                    {
                        BoardTileType.Nothing => throw new InvalidOperationException(),
                        BoardTileType.Ground => _viewConfig.Ground,
                        BoardTileType.Button => _viewConfig.Button,
                        BoardTileType.Spikes => _viewConfig.Spikes,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    var obj = InstantiateAt(tilePrefab, point);

                    _objects.Add(cell.Tile.Id, obj.transform);
                }

                if (cell.Object.Type != BoardObjectType.Nothing)
                {
                    var objPrefab = cell.Object.Type switch
                    {
                        BoardObjectType.Nothing => throw new InvalidOperationException(),
                        BoardObjectType.Character => _viewConfig.Character,
                        BoardObjectType.Obstacle => _viewConfig.Obstacle,
                        BoardObjectType.Box => _viewConfig.Box,
                        BoardObjectType.Coin => _viewConfig.Coin,
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    var obj = InstantiateAt(objPrefab, point);

                    _objects.Add(cell.Object.Id, obj.transform);
                }
            }
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

        private const float StepDuration = 0.5f;
        private float _timelinePos;

        private readonly List<MoveJob> _moveJobs = new List<MoveJob>();

        public void StartPlayingTimeline(List<TimeLineEvent> timeLine)
        {
            _timelinePos = 0;

            foreach (var evt in timeLine)
            {
                switch (evt.Type)
                {
                    case TimeLineEventType.Move:
                    {
                        var target = _objects[evt.EntityId];

                        var job = new MoveJob(
                            evt.Step * StepDuration,
                            evt.Value.Move.From,
                            evt.Value.Move.To,
                            target);

                        _moveJobs.Add(job);
                        break;
                    }
                    case TimeLineEventType.FlipFlop:
                    {
                        break;
                    }
                }
            }
        }

        public void Tick(float dt)
        {
            UpdateOrthographicSize();

            if (!IsPlayingTimeline)
                return;

            _timelinePos += dt;

            for (var i = 0; i < _moveJobs.Count; i++)
            {
                var job = _moveJobs[i];
                var finished = job.Update(_timelinePos, StepDuration);
                if (finished)
                    _moveJobs.RemoveAt(i);
            }
        }

        public bool IsPlayingTimeline => _moveJobs.Count > 0;

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