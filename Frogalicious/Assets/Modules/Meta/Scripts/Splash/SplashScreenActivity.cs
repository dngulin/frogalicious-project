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

        public SplashScreenActivity(in RootScope scope, SplashScreenUi uiPrefab)
        {
            _ui = Object.Instantiate(uiPrefab, scope.GameObjectStash);
        }

        public override void Dispose(in RootScope scope)
        {
            _ui.DestroyGameObject();
        }

        public override void Tick(in RootScope scope, float dt)
        {
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            using (scope.Ui.InstantUi(_ui))
            {
                await scope.Localization.LoadLanguage(scope.Save.Data.Language);
                var mainMenuRes = await MainMenuResources.Load(ct);

                await _ui.WaitForTap(ct);

                return Transition.Replace(new MainMenuActivity(scope, mainMenuRes));
            }
        }
    }
}