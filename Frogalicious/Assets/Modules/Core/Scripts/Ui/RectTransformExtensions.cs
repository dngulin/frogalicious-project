using UnityEngine;

namespace Frog.Core.Ui
{
    public static class RectTransformExtensions
    {
        public static void SetParentAndExpand(this RectTransform rt, Transform parent)
        {
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}