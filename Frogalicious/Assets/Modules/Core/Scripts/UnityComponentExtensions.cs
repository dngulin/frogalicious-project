using UnityEngine;

namespace Frog.Core
{
    public static class UnityComponentExtensions
    {
        public static void DestroyGameObject<T>(this T mb) where T : Component
        {
            if (mb != null)
                Object.Destroy(mb.gameObject);
        }
    }
}