using System.Threading;
using Frog.Core;
using Frog.Meta.MainMenu;
using Frog.StateTracker;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Meta.Splash
{
    public class SplashStateHandler : AsyncStateHandler<RootScope>
    {
        private readonly SplashUi _ui;
        private readonly AwaitableOperation _poll = new AwaitableOperation();

        private bool _enablePolling;

        public SplashStateHandler(SplashUi uiPrefab)
        {
            _ui = Object.Instantiate(uiPrefab);
        }

        public override void Dispose(in RootScope scope)
        {
            _poll.TryCancel();

            if (_ui != null)
                Object.Destroy(_ui);
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_enablePolling && _ui.Poll())
                _poll.EndAssertive();
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            var handle = scope.Ui.AddStaticWindow(_ui.transform);
            _enablePolling = true;

            await _poll.ExecuteAsync(ct);

            _enablePolling = false;
            scope.Ui.RemoveStaticWindow(handle);

            var menuGoPrefab = await Addressables.LoadAssetAsync<GameObject>("MainMenuUi.prefab").Task;
            return Transition.Replace(new MainMenuStateHandler(menuGoPrefab.GetComponent<MainMenuUi>()));
        }
    }
}