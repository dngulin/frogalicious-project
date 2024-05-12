using System;
using System.Collections.Generic;
using Frog.Collections;
using Frog.Core;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.Level.Simulation;
using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public class LevelView : IDisposable
    {
        private readonly Transform _root;
        private readonly Dictionary<ushort, EntityView> _objects = new Dictionary<ushort, EntityView>();

        private readonly LevelViewConfig _viewConfig;

        private OrthographicCameraFitter _cameraFitter;

        public LevelView(LevelViewConfig viewConfig, LevelData data, Camera camera)
        {
            var rootGo = new GameObject("LevelRoot");
            _root = rootGo.transform;

            camera.backgroundColor = viewConfig.BackgroundColor;

            _viewConfig = viewConfig;
            _cameraFitter = new OrthographicCameraFitter(camera, new Vector2(11, 7));
            _cameraFitter.SetPosition(new Vector2(data.Width - 1, data.Height - 1) * 0.5f);
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

        public void Dispose() => _root.DestroyGameObject();

        private float _timelinePos;
        private RefList<TimelineJob> _timeline = RefList.WithCapacity<TimelineJob>(32);

        public void StartPlayingTimeline(in RefList<TimeLineEvent> timeLineEvents)
        {
            _timelinePos = 0;

            for (var i = 0; i < timeLineEvents.Count(); i++)
            {
                ref readonly var evt = ref timeLineEvents.RefReadonlyAt(i);
                _timeline.RefAdd() = new TimelineJob(evt, _objects[evt.EntityId]);
            }
        }

        public void Tick(float dt)
        {
            _cameraFitter.UpdateSizeIfAspectChanged();

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
    }
}