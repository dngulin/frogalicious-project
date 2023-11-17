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
                    cell.transform.localPosition = new Vector2 { x = point.X, y = point.Y };
                }

                switch (cellData.ObjectType)
                {
                    case BoardObjectType.Character:
                        var hero = Object.Instantiate(viewConfig.Character, _root);
                        hero.transform.localPosition = new Vector2 { x = point.X, y = point.Y };
                        _character = hero.transform;
                        break;

                    case BoardObjectType.Obstacle:
                        var obstacle = Object.Instantiate(viewConfig.Obstacle, _root);
                        obstacle.transform.localPosition = new Vector2 { x = point.X, y = point.Y };
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (_root != null)
                Object.Destroy(_root.gameObject);
        }

        public void Tick(float dt)
        {

        }

        public bool IsBusy { get; private set; }

        public void DisplayEvents(List<SimulationEvent> events)
        {

        }
    }
}