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

        public LevelView(LevelViewConfig viewConfig, LevelData data)
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
        }

        private const float MoveDuration = 0.5f;
        private bool _moving;
        private float _moveTime;
        private Vector2 _moveStart;
        private Vector2 _moveEnd;

        public void Dispose()
        {
            if (_root != null)
                Object.Destroy(_root.gameObject);
        }

        public void Tick(float dt)
        {
            if (!_moving)
                return;

            _moveTime += dt;
            _character.localPosition = Vector2.Lerp(_moveStart, _moveEnd, _moveTime / MoveDuration);

            if (_moveTime >= MoveDuration)
                _moving = false;
        }

        public bool IsBusy => _moving;

        public void DisplayEvents(List<SimulationEvent> events)
        {
            foreach (var e in events)
            {
                if (e.Type == SimulationEventType.Move)
                {
                    _moving = true;
                    _moveTime = 0;
                    _moveStart = e.Position.ToVector2();
                    _moveEnd = e.EndPosition.ToVector2();
                }
            }
        }
    }
}