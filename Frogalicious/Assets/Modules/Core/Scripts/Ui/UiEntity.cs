using UnityEngine;

namespace Frog.Core.Ui
{
    public abstract class UiEntity : MonoBehaviour
    {
        public abstract void SetVisible(bool visible);
        public abstract CanvasGroup ContentsRoot { get; }
    }

    public static class UiEntityExtensions
    {
        public static void AttachContents(this UiEntity entity, Transform contents)
        {
            Debug.Assert(entity.ContentsRoot.transform.childCount == 0);
            contents.SetParent(entity.ContentsRoot.transform, false);
        }

        public static Transform DetachContents(this UiEntity entity, Transform contentsParent)
        {
            Debug.Assert(entity.ContentsRoot.transform.childCount == 1);
            var contents = entity.ContentsRoot.transform.GetChild(0);
            contents.SetParent(contentsParent, false);
            return contents;
        }

        public static void SetInteractable(this UiEntity entity, bool interactive)
        {
            entity.ContentsRoot.interactable = interactive;
        }
    }
}