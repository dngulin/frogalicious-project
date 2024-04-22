using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public sealed class UiEntityStatic : UiEntity
    {
        [SerializeField] private CanvasGroup _contentsRoot;

        private readonly AwaitableCompletionSource _acs = new AwaitableCompletionSource();

        public override void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public override Awaitable Show(CancellationToken _)
        {
            SetVisible(true);
            return CreateCompletedAwaitable();
        }

        public override Awaitable Hide(CancellationToken _)
        {
            SetVisible(false);
            return CreateCompletedAwaitable();
        }

        public override UiEntityState State => gameObject.activeSelf ? UiEntityState.Visible : UiEntityState.Hidden;

        public override CanvasGroup ContentsRoot => _contentsRoot;


        private Awaitable CreateCompletedAwaitable()
        {
            _acs.Reset();
            _acs.SetResult();
            return _acs.Awaitable;
        }

        public static UiEntityStatic Create()
        {
            var go = new GameObject(nameof(UiEntityStatic), typeof(RectTransform));
            var entity = go.AddComponent<UiEntityStatic>();
            entity._contentsRoot = go.AddComponent<CanvasGroup>();

            return entity;
        }
    }
}