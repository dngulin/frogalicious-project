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
            Debug.Assert(window.State == UiEntityState.Hidden);

            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
                await window.Show(ct);
                return (UiWindowHandle)handle;
            }
        }

        public async Awaitable<UiEntity> CloseWindow(UiWindowHandle handle, CancellationToken ct)
        {
            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var window = stackAccessor.RemoveItemAssertive((uint)handle);
                await window.Hide(ct);
                return window;
            }
        }

        public async Awaitable<UiDialogWindowHandle> OpenDialogWindow(Transform contents, CancellationToken ct)
        {
            var window = new GameObject(nameof(UiEntityStatic), typeof(RectTransform)).AddComponent<UiEntityStatic>(); // TODO: Pooling
            window.CreateContentsRoot();
            window.SetVisible(false);
            window.AttachContents(contents);

            return (UiDialogWindowHandle) await OpenWindow(window, ct);
        }

        public async Awaitable<Transform> CloseDialogWindow(UiDialogWindowHandle handle, CancellationToken ct)
        {
            var window = await CloseWindow((UiWindowHandle) handle, ct);
            window.DetachContents(out var contents);
            UnityEngine.Object.Destroy(window); // TODO: Pooling

            return contents;
        }

        public UiStaticWindowHandle AddStaticWindow(Transform contents)
        {
            var window = new GameObject(nameof(UiEntityStatic), typeof(RectTransform)).AddComponent<UiEntityStatic>(); // TODO: Pooling
            window.CreateContentsRoot();
            window.AttachContents(contents);

            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
                window.SetVisible(true);
                return (UiStaticWindowHandle)handle;
            }
        }

        public Transform RemoveStaticWindow(UiStaticWindowHandle handle)
        {
            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var window = stackAccessor.RemoveItemAssertive((uint)handle);
                window.DetachContents(out var contents);
                UnityEngine.Object.Destroy(window);

                return contents;
            }
        }
    }


    public enum UiWindowHandle : uint
    {
    }

    public enum UiDialogWindowHandle : uint
    {
    }

    public enum UiStaticWindowHandle : uint
    {
    }
}