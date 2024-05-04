using System;
using System.Threading;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Level.Data;
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
        private readonly MainMenuUi _menu;
        private readonly AwaitableOperation<MainMenuUi.Command> _uiPoll = new AwaitableOperation<MainMenuUi.Command>();

        public MainMenuStateHandler(in RootScope scope, MainMenuUi mainMenuPrefab)
        {
            _menu = UnityEngine.Object.Instantiate(mainMenuPrefab, scope.GameObjectStash);
        }

        public override void Dispose(in RootScope scope)
        {
            _uiPoll.Dispose();
            _menu.DestroyGameObject();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_menu.Poll().TryGetValue(out var command))
            {
                _uiPoll.EndAssertive(command);
            }
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            using (scope.Ui.FullscreenUi(_menu))
            {
                var command = await _uiPoll.ExecuteAsync(ct);
                switch (command)
                {
                    case MainMenuUi.Command.Play:
                        using (scope.Ui.LoadingUi())
                        {
                            var levelStateHandler = await CreateLevelStateHandler(scope, ct);
                            return Transition.Push(levelStateHandler);
                        }

                    case MainMenuUi.Command.Exit:
                        return Transition.Pop();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static async Awaitable<LevelStateHandler> CreateLevelStateHandler(RootScope scope, CancellationToken ct)
        {
            var data = await Addressables.LoadAssetAsync<LevelData>("Assets/Levels/Castle1.asset").Task;
            ct.ThrowIfCancellationRequested();

            var viewConfig = await Addressables.LoadAssetAsync<LevelViewConfig>("Assets/Modules/Level/Config/LevelViewConfig.asset").Task;
            ct.ThrowIfCancellationRequested();

            var panelPrefab = await Addressables.LoadAssetAsync<GameObject>("Assets/Modules/Level/Prefabs/Ui/LevelPanelUi.prefab").Task;
            ct.ThrowIfCancellationRequested();

            return new LevelStateHandler(scope, data, viewConfig, panelPrefab.GetComponent<LevelPanelUi>());
        }
    }
}