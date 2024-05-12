using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Meta.Level;
using Frog.StateTracker;
using UnityEngine;

namespace Frog.Meta.MainMenu
{
    public class MainMenuStateHandler : AsyncStateHandler<RootScope>
    {
        private MainMenuResources _res;

        private readonly MainMenuUi _menu;
        private readonly MainMenuView _view;

        private readonly AwaitableOperation<int> _waitLevelClick = new AwaitableOperation<int>();

        public MainMenuStateHandler(in RootScope scope, in MainMenuResources res)
        {
            _res = res;
            _menu = Object.Instantiate(res.MenuPrefab, scope.GameObjectStash);
            _view = new MainMenuView(scope.Camera, res.ChapterConfig.MapPrefab);
            scope.Camera.backgroundColor = res.ChapterConfig.BgColor;
        }

        public override void Dispose(in RootScope scope)
        {
            _waitLevelClick.Dispose();
            _menu.DestroyGameObject();
            _res.Dispose();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            _view.UpdateCamera();

            if (_view.PollLevelClick().TryGetValue(out var command))
            {
                _waitLevelClick.EndAssertive(command);
            }
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            _view.SetupCamera();
            _view.SetVisible(true);

            using (scope.Ui.InstantUi(_menu))
            {
                var levelIndex = await _waitLevelClick.ExecuteAsync(ct);
                using (scope.Ui.LoadingUi())
                {
                    var levelDataRef = _res.ChapterConfig.LevelList[levelIndex];
                    var resources = await LevelResources.Load(levelDataRef, ct);

                    _view.SetVisible(false);
                    return Transition.Push(new LevelStateHandler(scope, resources));
                }
            }
        }
    }
}