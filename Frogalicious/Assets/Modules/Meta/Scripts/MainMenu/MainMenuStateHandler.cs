using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Level.Ui;
using Frog.Level.View;
using Frog.Meta.Level;
using Frog.StateTracker;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Meta.MainMenu
{
    public class MainMenuStateHandler : AsyncStateHandler<RootScope>
    {
        private GameChapterConfig _chapterConfig;

        private readonly MainMenuUi _menu;
        private readonly MainMenuView _view;

        private readonly AwaitableOperation<int> _waitLevelClick = new AwaitableOperation<int>();

        public MainMenuStateHandler(in RootScope scope, MainMenuUi mainMenuPrefab, GameChapterConfig chapterConfig)
        {
            _chapterConfig = chapterConfig;

            _menu = Object.Instantiate(mainMenuPrefab, scope.GameObjectStash);
            _view = new MainMenuView(chapterConfig.MapPrefab);
            scope.Camera.backgroundColor = chapterConfig.BgColor;
        }

        public override void Dispose(in RootScope scope)
        {
            _waitLevelClick.Dispose();
            _menu.DestroyGameObject();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_view.PollLevelClick().TryGetValue(out var command))
            {
                _waitLevelClick.EndAssertive(command);
            }
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            using (scope.Ui.InstantUi(_menu))
            {
                var levelIndex = await _waitLevelClick.ExecuteAsync(ct);
                using (scope.Ui.LoadingUi())
                {
                    var levelStateHandler = await CreateLevelStateHandler(scope, levelIndex, ct);
                    return Transition.Push(levelStateHandler);
                }
            }
        }

        private async Awaitable<LevelStateHandler> CreateLevelStateHandler(RootScope scope, int levelIndex, CancellationToken ct)
        {
            var data = await _chapterConfig.LevelList[levelIndex].LoadAssetAsync().Task;
            ct.ThrowIfCancellationRequested();

            var viewConfig = await Addressables.LoadAssetAsync<LevelViewConfig>("Assets/Modules/Level/Config/LevelViewConfig.asset").Task;
            ct.ThrowIfCancellationRequested();

            var panelPrefab = await Addressables.LoadAssetAsync<GameObject>("Assets/Modules/Level/Prefabs/Ui/LevelPanelUi.prefab").Task;
            ct.ThrowIfCancellationRequested();

            return new LevelStateHandler(scope, data, viewConfig, panelPrefab.GetComponent<LevelPanelUi>());
        }
    }
}