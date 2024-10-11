using System;
using System.Threading;
using System.Threading.Tasks;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Meta.Level;
using Frog.ActivityTracker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Meta.MainMenu
{
    public class MainMenuActivity : AsyncActivity<RootScope>
    {
        private MainMenuResources _res;

        private readonly MainMenuUi _menu;
        private readonly MainMenuView _view;

        private readonly AwaitableOperation<MainMenuCommand> _waitCommand = new AwaitableOperation<MainMenuCommand>();

        private int _currLevelIdx;
        private Flag _waitLevelResultFlag;

        public MainMenuActivity(in RootScope scope, in MainMenuResources res)
        {
            _res = res;
            _menu = Object.Instantiate(res.MenuPrefab, scope.GameObjectStash);
            _view = new MainMenuView(scope.Camera, res.ChapterConfig.MapPrefab);
            scope.Camera.backgroundColor = res.ChapterConfig.BgColor;
        }

        public override void Dispose(in RootScope scope)
        {
            _waitCommand.Dispose();
            _menu.DestroyGameObject();
            _res.Dispose();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            _view.UpdateCamera();

            var optUiCommand = _menu.Poll();
            var optLevel = _view.PollLevelClick();

            if (optUiCommand.TryGetValue(out var uiCommand))
            {
                _waitCommand.EndAssertive(MainMenuCommand.FromUiCommand(uiCommand));
            }
            else if (optLevel.TryGetValue(out var levelIndex))
            {
                _waitCommand.EndAssertive(MainMenuCommand.PlayLevel(levelIndex));
            }
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            _view.SetupCamera();
            _view.SetVisible(true);

            if (CheckLevelCompletionFlags(scope.Mailbox))
            {
                _currLevelIdx = (_currLevelIdx + 1) % _res.ChapterConfig.LevelList.Length;
            }

            _view.SetCurrentLevel(_currLevelIdx);

            using (scope.Ui.InstantUi(_menu))
            {
                var command = await _waitCommand.ExecuteAsync(ct);
                return command.Id switch
                {
                    MainMenuCommandId.PlayLevel => await PlayLevel(scope, command.LevelIndex, ct),
                    MainMenuCommandId.Continue => await PlayLevel(scope, 0, ct),
                    MainMenuCommandId.ExitGame => Transition.Pop(),
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }

        private bool CheckLevelCompletionFlags(Mailbox mailbox)
        {
            var levelCompleted = mailbox.LevelCompletedFlag.TryReset();
            var waitLevelResult = _waitLevelResultFlag.TryReset();

            return levelCompleted && waitLevelResult;
        }

        private async Task<Transition> PlayLevel(RootScope scope, int levelIndex, CancellationToken ct)
        {
            using (scope.Ui.LoadingUi())
            {
                var levelDataRef = _res.ChapterConfig.LevelList[levelIndex];
                var resources = await LevelResources.Load(levelDataRef, ct);

                _view.SetVisible(false);
                _waitLevelResultFlag.SetIf(levelIndex == _currLevelIdx);

                return Transition.Push(new LevelActivity(scope, resources));
            }
        }
    }
}