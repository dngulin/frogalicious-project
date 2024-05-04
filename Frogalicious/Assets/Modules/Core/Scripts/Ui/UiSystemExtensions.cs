using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public static class UiSystemExtensions
    {
        public static FullScreenContainerHolder FullscreenUi<T>(this UiSystem ui, T contents) where T : MonoBehaviour
        {
            var parent = contents.transform.parent;
            var id = ui.ShowFullscreen(contents.transform);
            return new FullScreenContainerHolder(ui, id, parent);
        }

        public static LoadingWindowHolder LoadingUi(this UiSystem ui)
        {
            ui.ShowLoading();
            return new LoadingWindowHolder(ui);
        }

        public static async Awaitable<DynWindowHolder> AnimatedUi(this UiSystem ui, DynUiEntity entity, CancellationToken ct)
        {
            var parent = entity.transform.parent;
            var id = await ui.Show(entity, ct);
            return new DynWindowHolder(ui, id, parent, ct);
        }
    }

    public readonly struct FullScreenContainerHolder : IDisposable
    {
        private readonly UiSystem _ui;
        private readonly FullScreenUiEntityId _id;
        private readonly Transform _contentsParent;

        public FullScreenContainerHolder(UiSystem ui, FullScreenUiEntityId id, Transform contentsParent)
        {
            _ui = ui;
            _id = id;
            _contentsParent = contentsParent;
        }

        public void Dispose()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                _ui.HideFullscreen(_id, _contentsParent);
        }
    }

    public readonly struct LoadingWindowHolder : IDisposable
    {
        private readonly UiSystem _ui;
        public LoadingWindowHolder(UiSystem ui) => _ui = ui;
        public void Dispose()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                _ui.HideLoading();
        }
    }

    public readonly struct DynWindowHolder
    {
        private readonly UiSystem _ui;
        private readonly DynUiEntityId _id;
        private readonly Transform _parent;
        private readonly CancellationToken _ct;

        public DynWindowHolder(UiSystem ui, DynUiEntityId id, Transform parent, CancellationToken ct)
        {
            _ui = ui;
            _id = id;
            _parent = parent;
            _ct = ct;
        }

        public async Awaitable DisposeAsync()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                await _ui.Hide(_id, _parent, _ct);
        }
    }
}