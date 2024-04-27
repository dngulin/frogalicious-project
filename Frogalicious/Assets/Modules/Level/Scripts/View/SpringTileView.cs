using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.View
{
    public class SpringTileView : EntityView
    {
        public SpringTileView Initialized(in BoardDirection direction)
        {
            var angle = direction switch
            {
                BoardDirection.Up => 0f,
                BoardDirection.Right => 90f,
                BoardDirection.Down => 180f,
                BoardDirection.Left => 270f,
                _ => 0f,
            };
            transform.rotation = Quaternion.AngleAxis(angle, -Vector3.forward);

            return this;
        }
    }
}