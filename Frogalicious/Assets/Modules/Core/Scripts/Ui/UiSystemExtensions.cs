using System;
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
}