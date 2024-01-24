using System;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Level.View
{
    public static class LevelViewConfigObjectExtensions
    {
        public static EntityView CreateObject(this LevelViewConfig config, in ObjectState obj, Transform parent, in Vector2 position)
        {
            var rotation = Quaternion.identity;
            return obj.Type switch
            {
                BoardObjectType.Nothing => throw new InvalidOperationException(),
                BoardObjectType.Character => Object.Instantiate(config.Character, position, rotation, parent),
                BoardObjectType.Obstacle => Object.Instantiate(config.Obstacle, position, rotation, parent),
                BoardObjectType.Box => Object.Instantiate(config.Box, position, rotation, parent),
                BoardObjectType.Coin => Object.Instantiate(config.Coin, position, rotation, parent),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}