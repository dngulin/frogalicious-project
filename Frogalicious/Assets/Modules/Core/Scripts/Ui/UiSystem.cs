using System;
using UnityEngine;

namespace Frog.Core.Ui
{
    public class UiSystem : IDisposable
    {
        private UiStack _windows;

        public UiSystem(Canvas canvas)
        {
            _windows = new UiStack(nameof(_windows), canvas.transform);
        }

        public void Dispose()
        {
            _windows.Dispose();
        }

        public WindowId ShowWindow(UiEntity entity) => (WindowId)_windows.ShowImmediate(entity);

        public UiEntity HideWindow(WindowId id, Transform parent) => _windows.HideImmediate((UiEntityId)id, parent);


        public FullScreenWindowId ShowFullscreenWindow(Transform contents)
        {
            var window = UiEntityStatic.Create();
            window.AttachContents(contents);
            return (FullScreenWindowId)_windows.ShowImmediate(window);
        }

        public Transform HideFullscreenWindow(FullScreenWindowId id, Transform parent)
        {
            var window = _windows.HideImmediate((UiEntityId)id, parent);
            window.DetachContents(out var contents);
            window.DestroyGameObject();
            return contents;
        }
    }

    public enum WindowId : uint
    {
    }

    public enum FullScreenWindowId : uint
    {
    }
}