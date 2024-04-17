using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public enum UiEntityState
    {
        Appearing,
        Visible,
        Disappearing,
        Hidden,
    }

    public abstract class UiEntity : MonoBehaviour
    {
        public abstract void SetVisible(bool visible);
        public abstract Awaitable Show(CancellationToken ct);
        public abstract Awaitable Hide(CancellationToken ct);
        public abstract UiEntityState State { get; }

        public abstract CanvasGroup ContentsRoot { get; }
    }

    public static class UiEntityExtensions
    {
        public static void AttachContents(this UiEntity entity, Transform contents)
        {
            Debug.Assert(entity.ContentsRoot.transform.childCount == 0);
            contents.SetParent(entity.ContentsRoot.transform);
        }

        public static void DetachContents(this UiEntity entity, out Transform contents)
        {
            Debug.Assert(entity.ContentsRoot.transform.childCount == 1);
            contents = entity.ContentsRoot.transform.GetChild(0);
            contents.SetParent(null);
        }

        public static void SetInteractable(this UiEntity entity, bool interactive)
        {
            entity.ContentsRoot.interactable = interactive;
        }
    }
}