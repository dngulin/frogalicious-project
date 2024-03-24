using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Level.View
{
    public abstract class EntityView : MonoBehaviour
    {
        public virtual void FlipFlop(bool state) => throw new NotSupportedException();

        public virtual void Disappear() => throw new NotSupportedException();
    }

    public static class EntityViewExtensions
    {
        public static T Spawn<T>(this T prefab, Transform parent, Vector2 position) where T : EntityView
        {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }
    }
}