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
            switch (obj.Type)
            {
                case BoardObjectType.Nothing:
                    throw new InvalidOperationException();

                case BoardObjectType.Character:
                {
                    var character = Object.Instantiate(config.Prefabs.Character, position, Quaternion.identity, parent);
                    character.Init(config.Sprites.Character);
                    return character;
                }

                case BoardObjectType.Obstacle:
                {
                    var obstacle = Object.Instantiate(config.Prefabs.Obstacle, position, Quaternion.identity, parent);
                    obstacle.Init(config.Sprites.Obstacle);
                    return obstacle;
                }

                case BoardObjectType.Box:
                {
                    var box = Object.Instantiate(config.Prefabs.Box, position, Quaternion.identity, parent);
                    box.Init(config.Sprites.Box);
                    return box;
                }

                case BoardObjectType.Coin:
                {
                    var box = Object.Instantiate(config.Prefabs.Coin, position, Quaternion.identity, parent);
                    box.Init(config.Sprites.Coin);
                    return box;
                }


                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}