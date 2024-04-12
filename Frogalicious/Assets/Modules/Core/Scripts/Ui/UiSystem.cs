using System;
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

        public async Awaitable<UiWindowHandle> OpenWindow(Transform contents)
        {
            var window = new GameObject(nameof(UiEntityStatic), typeof(RectTransform)).AddComponent<UiEntityStatic>(); // TODO: Pooling
            window.CreateContentsRoot();
            window.SetVisible(false);

            Debug.Assert(window.State == UiEntityState.Hidden);
            window.AttachContents(contents);

            using var stackAccessor = _windowsStack.CreateAccessor();

            var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
            await window.Show();
            return (UiWindowHandle)handle;
        }

        public async Awaitable<Transform> CloseWindow(UiWindowHandle handle)
        {
            using var stackAccessor = _windowsStack.CreateAccessor();

            var windowFound = stackAccessor.RemoveItem((uint)handle, out var window);
            Debug.Assert(windowFound, $"No {nameof(window)} found with the handle {(uint)handle}");

            Debug.Assert(window.State == UiEntityState.Visible);
            await window.Hide();

            window.DetachContents(out var contents);
            UnityEngine.Object.Destroy(window); // TODO: Pooling

            return contents;
        }
    }


    public enum UiWindowHandle : uint
    {
    }
}