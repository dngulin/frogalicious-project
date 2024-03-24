using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.View
{
    public static class BoardPointExtensions
    {
        public static Vector2 ToVector2(this BoardPoint point) => new Vector2(point.X, point.Y);
    }
}