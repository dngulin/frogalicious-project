using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
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

        public SplashStateHandler(in RootScope scope, SplashUi uiPrefab)
        {
            _ui = Object.Instantiate(uiPrefab, scope.GameObjectStash);
        }

        public override void Dispose(in RootScope scope)
        {
            _poll.TryCancel();
            _ui.DestroyGameObject();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_poll.IsRunning && _ui.Poll())
                _poll.EndAssertive();
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            using (scope.Ui.FullscreenWindow(_ui.transform))
            {
                await _poll.ExecuteAsync(ct);

                using (scope.Ui.LoadingWindow())
                {
                    var menuGoPrefab = await Addressables.LoadAssetAsync<GameObject>("MainMenuUi.prefab").Task;
                    var stateHandler = new MainMenuStateHandler(scope, menuGoPrefab.GetComponent<MainMenuUi>());

                    return Transition.Replace(stateHandler);
                }
            }
        }
    }
}