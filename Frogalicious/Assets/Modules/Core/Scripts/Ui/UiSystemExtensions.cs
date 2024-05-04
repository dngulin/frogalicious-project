using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core.Ui
{
    public static class UiSystemExtensions
    {
        public static UiEntityHolder InstantUi(this UiSystem ui, UiEntity entity)
        {
            var parent = entity.transform.parent;
            var id = ui.ShowInstant(entity);
            return new UiEntityHolder(ui, id, parent);
        }

        public static async Awaitable<AnimatedUiEntityHolder> AnimatedUi(this UiSystem ui, AnimatedUiEntity entity, CancellationToken ct)
        {
            var parent = entity.transform.parent;
            var id = await ui.Show(entity, ct);
            return new AnimatedUiEntityHolder(ui, id, parent, ct);
        }

        public static LoadingUiHolder LoadingUi(this UiSystem ui)
        {
            ui.ShowLoading();
            return new LoadingUiHolder(ui);
        }
    }

    public readonly struct UiEntityHolder : IDisposable
    {
        private readonly UiSystem _ui;
        private readonly UiEntityId _id;
        private readonly Transform _parent;

        public UiEntityHolder(UiSystem ui, UiEntityId id, Transform parent)
        {
            _ui = ui;
            _id = id;
            _parent = parent;
        }

        public void Dispose()
        {
            if (_ui.IsUnderlyingGameObjectAlive)
                _ui.HideInstant(_id, _parent);
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
}