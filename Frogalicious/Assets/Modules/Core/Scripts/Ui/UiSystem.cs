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

        public async Awaitable<AnimatedUiEntityHandle> OpenWindow(AnimatedUiEntity window, CancellationToken ct)
        {
            Debug.Assert(window.State == AnimatedUiEntityState.Hidden);

            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
                await window.Show(ct);
                return (AnimatedUiEntityHandle) handle;
            }
        }

        public async Awaitable<AnimatedUiEntity> CloseWindow(AnimatedUiEntityHandle handle, CancellationToken ct)
        {
            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var window = (AnimatedUiEntity) stackAccessor.RemoveItemAssertive((uint)handle);
                await window.Hide(ct);
                return window;
            }
        }

        public UiEntityHandle AddWindow(UiEntity window)
        {
            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
                window.SetVisible(true);
                return (UiEntityHandle) handle;
            }
        }

        public UiEntity RemoveWindow(UiEntityHandle handle)
        {
            using (var stackAccessor = _windowsStack.CreateAccessor())
            {
                var window = stackAccessor.RemoveItemAssertive((uint)handle);
                window.SetVisible(false);
                return window;
            }
        }

        public UiEntityHandle AddStaticWindow(Transform contents)
        {
            var window = UiEntityStatic.Create();
            window.AttachContents(contents);

            return AddWindow(window);
        }

        public Transform RemoveStaticWindow(UiEntityHandle handle)
        {
            var window = RemoveWindow(handle);
            window.DetachContents(out var contents);
            window.DestroyGameObject();

            return contents;
        }
    }
}