using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frog.Core.Ui
{
    public readonly struct UiStack : IDisposable
    {
        private readonly GameObject _root;
        private readonly List<(uint, UiEntity)> _items;

        public UiStack(string name, Transform parent, int capacity = 16)
        {
            Debug.Assert(parent.GetComponent<Canvas>() != null);

            _root = new GameObject(name, typeof(RectTransform));
            _root.GetComponent<RectTransform>().SetParentAndExpand(parent);

            _items = new List<(uint, UiEntity)>(capacity);
        }

        public void Dispose()
        {
            if (_root != null)
                UnityEngine.Object.Destroy(_root);
        }

        public UiStackAccessor CreateAccessor() => new UiStackAccessor(_root.transform, _items);
    }

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

        public bool RemoveItem(uint handle, out UiEntity window)
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
}