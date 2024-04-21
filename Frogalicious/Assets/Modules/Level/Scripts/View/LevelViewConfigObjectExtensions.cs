using System;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Level.View
{
    public static class LevelViewConfigObjectExtensions
    {
        public static EntityView CreateObject(this LevelViewConfig config, in LevelState state, in BoardPoint point, Transform parent)
        {
            ref readonly var obj = ref state.Cells.RefReadonlyAt(point).Object;
            return obj.Type switch
            {
                BoardObjectType.Nothing => throw new InvalidOperationException(),
                BoardObjectType.Character => Spawn(config.Character, point, parent),
                BoardObjectType.Obstacle => SpawnObstacle(config, state, point, parent),
                BoardObjectType.Box => Spawn(config.Box, point, parent),
                BoardObjectType.Coin => Spawn(config.Coin, point, parent),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
        
        private static T Spawn<T>(T prefab, in BoardPoint p, Transform parent) where T : Object
        {
            return Object.Instantiate(prefab, p.ToVector2(), Quaternion.identity, parent);
        }

        private static EntityView SpawnObstacle(LevelViewConfig config, in LevelState state, in BoardPoint point, Transform parent)
        {
            var l = HasObj(state.Cells, BoardObjectType.Obstacle, point + BoardPoint.Left);
            var r = HasObj(state.Cells, BoardObjectType.Obstacle, point + BoardPoint.Right);
            return Spawn(config.Obstacle, point, parent).Initialized(l, r);
        }

        private static bool HasObj(in CellsState cells, BoardObjectType type, in BoardPoint point)
        {
            return cells.HasPoint(point) && cells.RefReadonlyAt(point).Object.Type == type;
        }
    }
}