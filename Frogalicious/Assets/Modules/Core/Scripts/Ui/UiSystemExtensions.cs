using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public static class UiSystemExtensions
    {
        public static FullScreenContainerHolder FullscreenContainer(this UiSystem ui, Transform contents)
        {
            var parent = contents.parent;
            var id = ui.ShowFullscreenContainerWith(contents);
            return new FullScreenContainerHolder(ui, id, parent);
        }

        public static LoadingWindowHolder LoadingSplash(this UiSystem ui)
        {
            ui.ShowLoading();
            return new LoadingWindowHolder(ui);
        }

        public static async Awaitable<DynWindowHolder> DynWindow(this UiSystem ui, DynUiEntity entity, CancellationToken ct)
        {
            var parent = entity.transform.parent;
            var id = await ui.ShowWindow(entity, ct);
            return new DynWindowHolder(ui, id, parent, ct);
        }
    }

    public readonly struct FullScreenContainerHolder : IDisposable
    {
        private readonly UiSystem _ui;
        private readonly FullScreenContainerId _id;
        private readonly Transform _contentsParent;

        public FullScreenContainerHolder(UiSystem ui, FullScreenContainerId id, Transform contentsParent)
        {
            _ui = ui;
            _id = id;
            _contentsParent = contentsParent;
        }

        public void Dispose() => _ui.HideFullscreenContainer(_id, _contentsParent);
    }

    public readonly struct LoadingWindowHolder : IDisposable
    {
        private readonly UiSystem _ui;
        public LoadingWindowHolder(UiSystem ui) => _ui = ui;
        public void Dispose() => _ui.HideLoading();
    }

    public readonly struct DynWindowHolder
    {
        private readonly UiSystem _ui;
        private readonly DynWindowId _id;
        private readonly Transform _parent;
        private readonly CancellationToken _ct;

        public DynWindowHolder(UiSystem ui, DynWindowId id, Transform parent, CancellationToken ct)
        {
            _ui = ui;
            _id = id;
            _parent = parent;
            _ct = ct;
        }

        public async Awaitable DisposeAsync()
        {
            await _ui.HideWindow(_id, _parent, _ct);
        }
    }
}