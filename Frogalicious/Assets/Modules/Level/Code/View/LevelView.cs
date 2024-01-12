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
        private readonly Dictionary<BoardPoint, Transform> _objects = new Dictionary<BoardPoint, Transform>();

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
                        _ => throw new ArgumentOutOfRangeException(),
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
                        _ => throw new ArgumentOutOfRangeException(),
                    };
                    var obj = InstantiateAt(objPrefab, point);

                    _objects.Add(point, obj.transform);
                }
            }


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

        private const float StepDuration = 0.5f;
        private float _timelinePos;

        private readonly List<MoveJob> _moveJobs = new List<MoveJob>();

        public void StartPlayingTimeline(List<TimeLineEvent> timeLine)
        {
            _timelinePos = 0;

            foreach (var evt in timeLine)
            {
                if (evt.Type == TimeLineEventType.Move)
                {
                    var target = _objects[evt.Position];
                    _objects.Remove(evt.Position);

                    var job = new MoveJob(
                        evt.Step * StepDuration,
                        evt.Position,
                        evt.EndPosition,
                        target);

                    _moveJobs.Add(job);
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
                {
                    _objects.Add(job.EndPos, job.Target);
                    _moveJobs.RemoveAt(i);
                }
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