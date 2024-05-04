using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public static class UiSystemExtensions
    {
        public static FullScreenUiHolder FullscreenUi<T>(this UiSystem ui, T contents) where T : MonoBehaviour
        {
            var parent = contents.transform.parent;
            var id = ui.ShowFullscreen(contents.transform);
            return new FullScreenUiHolder(ui, id, parent);
        }

        public static LoadingUiHolder LoadingUi(this UiSystem ui)
        {
            ui.ShowLoading();
            return new LoadingUiHolder(ui);
        }

        public static async Awaitable<AnimatedUiEntityHolder> AnimatedUi(this UiSystem ui, AnimatedUiEntity entity, CancellationToken ct)
        {
            var parent = entity.transform.parent;
            var id = await ui.Show(entity, ct);
            return new AnimatedUiEntityHolder(ui, id, parent, ct);
        }
    }

    public readonly struct FullScreenUiHolder : IDisposable
    {
        private readonly UiSystem _ui;
        private readonly FullScreenUiEntityId _id;
        private readonly Transform _contentsParent;

        public FullScreenUiHolder(UiSystem ui, FullScreenUiEntityId id, Transform contentsParent)
        {
            _ui = ui;
            _id = id;
            _contentsParent = contentsParent;
        }

        public void Dispose()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                _ui.HideFullscreen(_id, _contentsParent);
        }
    }

    public readonly struct LoadingUiHolder : IDisposable
    {
        private readonly UiSystem _ui;
        public LoadingUiHolder(UiSystem ui) => _ui = ui;
        public void Dispose()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                _ui.HideLoading();
        }
    }

    public readonly struct AnimatedUiEntityHolder
    {
        private readonly UiSystem _ui;
        private readonly AnimatedUiEntityId _id;
        private readonly Transform _parent;
        private readonly CancellationToken _ct;

        public AnimatedUiEntityHolder(UiSystem ui, AnimatedUiEntityId id, Transform parent, CancellationToken ct)
        {
            _ui = ui;
            _id = id;
            _parent = parent;
            _ct = ct;
        }

        public async Awaitable DisposeAsync()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                await _ui.Hide(_id, _parent, _ct);
        }
    }
}