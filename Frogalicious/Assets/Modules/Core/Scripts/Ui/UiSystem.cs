using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public class UiSystem : IDisposable
    {
        private UiStack _uiStack;

        private readonly Transform _loadingUiParent;
        private readonly UiEntity _loadingUi;
        private UiEntityId? _loadingWindowId;

        public UiSystem(Canvas canvas, UiEntity loadingUi)
        {
            _uiStack = new UiStack(nameof(_uiStack), canvas.transform);
            _loadingUiParent = loadingUi.transform.parent;
            _loadingUi = loadingUi;
        }

        public void Dispose()
        {
            _uiStack.Dispose();
            _loadingUi.DestroyGameObject();
        }

        public bool IsUnderlyingGameObjectAlive => _uiStack.IsUnderlyingGameObjectAlive;

        public Awaitable<AnimatedUiEntityId> Show(AnimatedUiEntity entity, CancellationToken ct)
        {
            return _uiStack.Show(entity, ct);
        }

        public Awaitable<AnimatedUiEntity> Hide(AnimatedUiEntityId id, Transform parent, CancellationToken ct)
        {
            return _uiStack.Hide(id, parent, ct);
        }

        public UiEntityId ShowInstant(UiEntity entity) => _uiStack.ShowInstant(entity);

        public UiEntity HideInstant(UiEntityId id, Transform parent) => _uiStack.HideInstant(id, parent);

        public void ShowLoading()
        {
            if (_loadingWindowId.HasValue)
                throw new InvalidOperationException();

            _loadingWindowId = _uiStack.ShowInstant(_loadingUi);
        }

        public void HideLoading()
        {
            if (!_loadingWindowId.HasValue)
                throw new InvalidOperationException();

            _uiStack.HideInstant(_loadingWindowId.Value, _loadingUiParent);
            _loadingWindowId = null;
        }
    }
}