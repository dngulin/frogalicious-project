using System;
using UnityEngine;

namespace Frog.Core.Ui
{
    public static class UiSystemExtensions
    {
        public static FullScreenWindowHolder FullscreenWindow(this UiSystem ui, Transform contents)
        {
            var parent = contents.parent;
            var id = ui.ShowFullscreenWindow(contents);
            return new FullScreenWindowHolder(ui, id, parent);
        }

        public static LoadingWindowHolder LoadingWindow(this UiSystem ui)
        {
            ui.ShowLoading();
            return new LoadingWindowHolder(ui);
        }
    }

    public readonly struct FullScreenWindowHolder : IDisposable
    {
        private readonly UiSystem _ui;
        private readonly FullScreenWindowId _id;
        private readonly Transform _contentsParent;

        public FullScreenWindowHolder(UiSystem ui, FullScreenWindowId id, Transform contentsParent)
        {
            _ui = ui;
            _id = id;
            _contentsParent = contentsParent;
        }

        public void Dispose() => _ui.HideFullscreenWindow(_id, _contentsParent);
    }

    public readonly struct LoadingWindowHolder : IDisposable
    {
        private readonly UiSystem _ui;
        public LoadingWindowHolder(UiSystem ui) => _ui = ui;
        public void Dispose() => _ui.HideLoading();
    }
}