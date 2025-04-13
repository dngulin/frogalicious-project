using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Frog.Core.Ui
{
    public class UiStack : IDisposable
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

        public bool IsUnderlyingGameObjectAlive => _root != null;

        public async Awaitable<AnimatedUiEntityId> Show(AnimatedUiEntity entity, CancellationToken ct)
        {
            Debug.Assert(entity.State == AnimatedUiEntityState.Disappeared, entity.State);
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var entityId = _nextUiEntityId++;
                stackAccessor.AddItem(entity, entityId);

                entity.SetInteractable(false);
                await entity.Show(ct);

                return (AnimatedUiEntityId)entityId;
            }
        }

        public async Awaitable<AnimatedUiEntity> Hide(AnimatedUiEntityId id, Transform parent, CancellationToken ct)
        {
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var entity = (AnimatedUiEntity)stackAccessor.FindItemAssertive((uint)id);
                Debug.Assert(entity.State == AnimatedUiEntityState.Appeared, entity.State);

                if (entity.IsGameObjectAlive())
                    await entity.Hide(ct);

                var removedEntity = stackAccessor.RemoveItemAssertive((uint)id, parent);
                Debug.Assert(ReferenceEquals(entity, removedEntity));

                return entity;
            }
        }

        public UiEntityId ShowInstant(UiEntity entity)
        {
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var entityId = _nextUiEntityId++;
                stackAccessor.AddItem(entity, entityId);
                entity.SetVisible(true);
                return (UiEntityId)entityId;
            }
        }

        public UiEntity HideInstant(UiEntityId id, Transform parent)
        {
            using (var stackAccessor = new UiStackAccessor(_root, _items))
            {
                var entity = stackAccessor.RemoveItemAssertive((uint)id, parent);

                if (entity.IsGameObjectAlive())
                    entity.SetVisible(false);

                return entity;
            }
        }
    }

    public enum UiEntityId : uint
    {
    }

    public enum AnimatedUiEntityId : uint
    {
    }
}