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

        public UiEntityHandle ShowWindow(UiEntity entity) => _windows.ShowImmediate(entity);

        public UiEntity HideWindow(UiEntityHandle handle) => _windows.HideImmediate(handle);


        public UiEntityHandle ShowFullscreenWindow(Transform contents)
        {
            var window = UiEntityStatic.Create();
            window.AttachContents(contents);
            return _windows.ShowImmediate(window);
        }

        public Transform HideFullscreenWindow(UiEntityHandle handle)
        {
            var window = _windows.HideImmediate(handle);
            window.DetachContents(out var contents);
            window.DestroyGameObject();
            return contents;
        }
    }
}