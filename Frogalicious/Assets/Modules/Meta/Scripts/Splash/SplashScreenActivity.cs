using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Meta.MainMenu;
using Frog.ActivityTracker;
using UnityEngine;

namespace Frog.Meta.Splash
{
    public class SplashScreenActivity : AsyncActivity<RootScope>
    {
        private readonly SplashScreenUi _ui;
        private readonly AwaitableOperation _poll = new AwaitableOperation();

        public SplashScreenActivity(in RootScope scope, SplashScreenUi uiPrefab)
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
            using (scope.Ui.InstantUi(_ui))
            {
                await _poll.ExecuteAsync(ct);

                using (scope.Ui.LoadingUi())
                {
                    var resources = await MainMenuResources.Load(ct);
                    return Transition.Replace(new MainMenuActivity(scope, resources));
                }
            }
        }
    }
}