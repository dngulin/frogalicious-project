using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frog.Core.Ui
{
    public readonly struct UiStackAccessor : IDisposable
    {
        private readonly Transform _root;
        private readonly List<(uint, UiEntity)> _entities;

        public UiStackAccessor(Transform root, List<(uint, UiEntity)> entities)
        {
            _root = root;
            _entities = entities;

            SetTopWindowInteractable(false);
            CheckConsistency();
        }

        public void Dispose()
        {
            SetTopWindowInteractable(true);
            CheckConsistency();
        }

        [Conditional("UNITY_ASSERTIONS")]
        private void CheckConsistency()
        {
            Debug.Assert(
                _root.childCount == _entities.Count,
                "Ui stack is in an inconsistent state (items count)"
            );
        }

        private void SetTopWindowInteractable(bool interactable)
        {
            if (_entities.Count == 0)
                return;

            var (_, window) = _entities[^1];
            window.SetInteractable(interactable);
        }

        public uint AddItem(UiEntity window, ref uint nextEntityId)
        {
            var handle = nextEntityId++;
            _entities.Add((handle, window));
            window.GetComponent<RectTransform>().SetParentAndExpand(_root);
            return handle;
        }

        public bool TryRemoveItem(uint handle, out UiEntity window)
        {
            for (var i = 0; i < _entities.Count; i++)
            {
                var (h, w) = _entities[i];
                if (h != handle)
                    continue;

                _entities.RemoveAt(i);
                window = w;
                window.transform.SetParent(null);
                return true;
            }

            window = default;
            return false;
        }
    }

    public static class UiStackAccessorExtensions
    {
        public static UiEntity RemoveItemAssertive(in this UiStackAccessor stackAccessor, uint handle)
        {
            var itemFound = stackAccessor.TryRemoveItem(handle, out var item);
            Debug.Assert(itemFound, $"No {nameof(item)} found with the handle {handle}");

            return item;
        }
    }
}