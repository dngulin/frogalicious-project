using UnityEngine;

namespace Frog.Core
{
    public static class UnityComponentExtensions
    {
        public static void DestroyGameObject<T>(this T component) where T : Component
        {
            if (component != null)
                Object.Destroy(component.gameObject);
        }

        public static bool IsGameObjectAlive<T>(this T component) where T : Component
        {
            return component.gameObject != null;
        }
    }
}