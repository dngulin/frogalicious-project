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
        private readonly List<(uint, UiEntity)> _items;

        public UiStackAccessor(Transform root, List<(uint, UiEntity)> items)
        {
            _root = root;
            _items = items;

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
                _root.childCount == _items.Count,
                "Ui stack is in an inconsistent state (items count)"
            );
        }

        private void SetTopWindowInteractable(bool interactable)
        {
            if (_items.Count == 0)
                return;

            var (_, entity) = _items[^1];
            entity.SetInteractable(interactable);
        }

        public void AddItem(UiEntity entity, uint entityId)
        {
            entity.GetComponent<RectTransform>().SetParentAndExpand(_root);

            var item = (entityId, entity);
            _items.Add(item);
        }

        public bool TryRemoveItem(uint entityId, out UiEntity result, Transform parent)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var (id, entity) = _items[i];
                if (id != entityId)
                    continue;

                _items.RemoveAt(i);
                result = entity;
                result.transform.SetParent(parent, false);
                return true;
            }

            result = default;
            return false;
        }
    }

    public static class UiStackAccessorExtensions
    {
        public static UiEntity RemoveItemAssertive(in this UiStackAccessor accessor, uint handle, Transform parent)
        {
            var itemFound = accessor.TryRemoveItem(handle, out var item, parent);
            Debug.Assert(itemFound, $"No {nameof(item)} found with the handle {handle}");

            return item;
        }
    }
}