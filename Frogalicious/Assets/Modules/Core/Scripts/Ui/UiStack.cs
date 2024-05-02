using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frog.Core.Ui
{
    public readonly struct UiStack : IDisposable
    {
        private readonly Transform _root;
        private readonly List<(uint, UiEntity)> _items;

        public UiStack(string name, Transform parent, int capacity = 16)
        {
            Debug.Assert(parent.GetComponent<Canvas>() != null);

            _root = new GameObject(name, typeof(RectTransform)).transform;
            _root.GetComponent<RectTransform>().SetParentAndExpand(parent);

            _items = new List<(uint, UiEntity)>(capacity);
        }

        public void Dispose() => _root.DestroyGameObject();

        public UiStackAccessor CreateAccessor() => new UiStackAccessor(_root, _items);
    }
}