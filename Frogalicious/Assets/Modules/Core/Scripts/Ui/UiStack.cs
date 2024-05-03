using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frog.Core.Ui
{
    public struct UiStack : IDisposable
    {
        private readonly Transform _root;
        private readonly List<(uint, UiEntity)> _items;

        private uint _nextUiEntityId;

        public UiStack(string name, Transform parent, int capacity = 16)
        {
            Debug.Assert(parent.GetComponent<Canvas>() != null);

            _root = new GameObject(name, typeof(RectTransform)).transform;
            _root.GetComponent<RectTransform>().SetParentAndExpand(parent);

            _items = new List<(uint, UiEntity)>(capacity);
            _nextUiEntityId = 0;
        }

        public void Dispose() => _root.DestroyGameObject();

        public async Awaitable<AnimatedUiEntityHandle> Show(AnimatedUiEntity window, CancellationToken ct)
        {
            Debug.Assert(window.State == AnimatedUiEntityState.Hidden);

            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
                await window.Show(ct);
                return (AnimatedUiEntityHandle) handle;
            }
        }

        public async Awaitable<AnimatedUiEntity> Hide(AnimatedUiEntityHandle handle, Transform parent, CancellationToken ct)
        {
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var window = (AnimatedUiEntity) stackAccessor.RemoveItemAssertive((uint)handle, parent);
                await window.Hide(ct);
                return window;
            }
        }

        public UiEntityHandle ShowImmediate(UiEntity window)
        {
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var handle = stackAccessor.AddItem(window, ref _nextUiEntityId);
                window.SetVisible(true);
                return (UiEntityHandle) handle;
            }
        }

        public UiEntity HideImmediate(UiEntityHandle handle, Transform parent)
        {
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var window = stackAccessor.RemoveItemAssertive((uint)handle, parent);
                window.SetVisible(false);
                return window;
            }
        }
    }
}