using System;
using UnityEngine;

namespace Frog.Core.Ui
{
    public static class UiSystemExtensions
    {
        public static FullScreenWindowHolder FullscreenWindow(this UiSystem ui, Transform contents)
        {
            return new FullScreenWindowHolder(ui, contents);
        }
    }

    public readonly struct FullScreenWindowHolder : IDisposable
    {
        private readonly UiSystem _ui;
        private readonly Transform _contentsParent;
        private readonly FullScreenWindowId _id;

        public FullScreenWindowHolder(UiSystem ui, Transform contents)
        {
            _ui = ui;
            _contentsParent = contents.parent;
            _id = ui.ShowFullscreenWindow(contents);
        }

        public void Dispose() => _ui.HideFullscreenWindow(_id, _contentsParent);
    }
}