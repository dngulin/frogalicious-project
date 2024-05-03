using System;
using UnityEngine;

namespace Frog.Core.Ui
{
    public class UiSystem : IDisposable
    {
        private UiStack _windows;

        private readonly Transform _loadingUiParent;
        private readonly UiEntity _loadingUi;
        private UiEntityId? _loadingWindowId;

        public UiSystem(Canvas canvas, UiEntity loadingUi)
        {
            _windows = new UiStack(nameof(_windows), canvas.transform);
            _loadingUiParent = loadingUi.transform.parent;
            _loadingUi = loadingUi;
        }

        public void Dispose()
        {
            _windows.Dispose();
            _loadingUi.DestroyGameObject();
        }

        public WindowId ShowWindow(UiEntity entity) => (WindowId)_windows.ShowImmediate(entity);

        public UiEntity HideWindow(WindowId id, Transform parent) => _windows.HideImmediate((UiEntityId)id, parent);


        public FullScreenWindowId ShowFullscreenWindow(Transform contents)
        {
            var window = UiEntityStatic.Create();
            window.AttachContents(contents);
            return (FullScreenWindowId)_windows.ShowImmediate(window);
        }

        public Transform HideFullscreenWindow(FullScreenWindowId id, Transform contentsParent)
        {
            var window = _windows.HideImmediate((UiEntityId)id, null);
            var contents = window.DetachContents(contentsParent);
            window.DestroyGameObject();
            return contents;
        }

        public void ShowLoading()
        {
            if (_loadingWindowId.HasValue)
                throw new InvalidOperationException();

            _loadingWindowId = _windows.ShowImmediate(_loadingUi);
        }

        public void HideLoading()
        {
            if (!_loadingWindowId.HasValue)
                throw new InvalidOperationException();

            _windows.HideImmediate(_loadingWindowId.Value, _loadingUiParent);
            _loadingWindowId = null;
        }
    }

    public enum WindowId : uint
    {
    }

    public enum FullScreenWindowId : uint
    {
    }
}