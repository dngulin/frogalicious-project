using System;
using System.Threading;
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

        public async Awaitable<DynWindowId> ShowWindow(DynUiEntity entity, CancellationToken ct)
        {
            return (DynWindowId) await _windows.Show(entity, ct);
        }

        public Awaitable<DynUiEntity> HideWindow(DynWindowId id, Transform parent, CancellationToken ct)
        {
            return _windows.Hide((DynUiEntityId)id, parent, ct);
        }

        public WindowId ShowWindow(UiEntity entity) => (WindowId)_windows.ShowImmediate(entity);

        public UiEntity HideWindow(WindowId id, Transform parent) => _windows.HideImmediate((UiEntityId)id, parent);


        public FullScreenContainerId ShowFullscreenContainerWith(Transform contents)
        {
            var container = StaticUiEntity.Create();
            container.AttachContent(contents);
            return (FullScreenContainerId)_windows.ShowImmediate(container);
        }

        public Transform HideFullscreenContainer(FullScreenContainerId id, Transform contentsParent)
        {
            var window = (StaticUiEntity)_windows.HideImmediate((UiEntityId)id, null);
            var contents = window.DetachContent(contentsParent);
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

    public enum FullScreenContainerId : uint
    {
    }

    public enum DynWindowId : uint
    {
    }
}