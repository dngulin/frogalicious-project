using System;
using System.Collections.Generic;
using Frog.Collections;
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
        private readonly Dictionary<ushort, EntityView> _objects = new Dictionary<ushort, EntityView>();

        private readonly LevelViewConfig _viewConfig;
        private readonly Camera _camera;

        public LevelView(LevelViewConfig viewConfig, LevelData data, Camera camera)
        {
            var rootGo = new GameObject("LevelRoot");
            _root = rootGo.transform;

            var center = new Vector2(data.Width - 1, data.Height - 1) * 0.5f;
            camera.transform.position = new Vector3(center.x, center.y, -10);
            camera.backgroundColor = viewConfig.BackgroundColor;

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
                    var tile = _viewConfig.CreateTile(cell.Tile, _root, point.ToVector2());
                    _objects.Add(cell.Tile.Id, tile);
                }

                if (cell.Object.Type != BoardObjectType.Nothing)
                {
                    var obj = _viewConfig.CreateObject(state, point, _root);
                    _objects.Add(cell.Object.Id, obj);
                }
            }
        }

        public void Dispose()
        {
            if (_root != null)
                Object.Destroy(_root.gameObject);
        }

        private float _timelinePos;
        private RefList<TimelineJob> _timeline = RefList.WithCapacity<TimelineJob>(32);

        public void StartPlayingTimeline(in RefList<TimeLineEvent> timeLineEvents)
        {
            _timelinePos = 0;

            for (var i = 0; i < timeLineEvents.Count(); i++)
            {
                ref readonly var evt = ref timeLineEvents.RefReadonlyAt(i);
                _timeline.Add(new TimelineJob(evt, _objects[evt.EntityId]));
            }
        }

        public void Tick(float dt)
        {
            UpdateOrthographicSize();

            if (!IsPlayingTimeline)
                return;

            for (var i = 0; i < _timeline.Count(); i++)
            {
                var finished = _timeline.RefReadonlyAt(i).Update(_timelinePos, dt);
                if (finished)
                {
                    _timeline.RemoveAt(i);
                    i--;
                }
            }

            _timelinePos += dt;
        }

        public bool IsPlayingTimeline => _timeline.Count() > 0;

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