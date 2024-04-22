using System;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frog.Core.Ui
{
    public class UiSystem : IDisposable
    {
        private uint _nextUiEntityId;
        private readonly UiStack _windowsStack;

        public UiSystem(Canvas canvas)
        {
            _windowsStack = new UiStack(nameof(_windowsStack), canvas.transform);
        }

        public void Dispose()
        {
            _windowsStack.Dispose();
        }

        public async Awaitable<UiWindowHandle> OpenWindow(UiEntity window, CancellationToken ct)
        {
            using var stackAccessor = _windowsStack.CreateAccessor();

            var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
            await window.Show(ct);
            return (UiWindowHandle)handle;
        }

        public async Awaitable<UiEntity> CloseWindow(UiWindowHandle handle, CancellationToken ct)
        {
            using var stackAccessor = _windowsStack.CreateAccessor();

            var windowFound = stackAccessor.RemoveItem((uint)handle, out var window);
            Debug.Assert(windowFound, $"No {nameof(window)} found with the handle {(uint)handle}");

            Debug.Assert(window.State == UiEntityState.Visible);
            await window.Hide(ct);

            return window;
        }

        public async Awaitable<UiDialogHandle> OpenDialogWindow(Transform contents, CancellationToken ct)
        {
            var window = new GameObject(nameof(UiEntityStatic), typeof(RectTransform)).AddComponent<UiEntityStatic>(); // TODO: Pooling
            window.CreateContentsRoot();
            window.SetVisible(false);

            Debug.Assert(window.State == UiEntityState.Hidden);
            window.AttachContents(contents);

            return (UiDialogHandle) await OpenWindow(window, ct);
        }

        public async Awaitable<Transform> CloseDialogWindow(UiDialogHandle handle, CancellationToken ct)
        {
            var window = await CloseWindow((UiWindowHandle) handle, ct);

            window.DetachContents(out var contents);
            UnityEngine.Object.Destroy(window); // TODO: Pooling

            return contents;
        }
    }


    public enum UiWindowHandle : uint
    {
    }

    public enum UiDialogHandle : uint
    {
    }
}