using System;
using System.Threading;
using Frog.Core;
using Frog.Meta.Level;
using Frog.StateTracker;
using UnityEngine;

namespace Frog.Meta.MainMenu
{
    public class MainMenuStateHandler : AsyncStateHandler<RootScope>
    {
        private readonly MainMenuUi _menu;
        private readonly AwaitableOperation<MainMenuUi.Command> _uiPoll = new AwaitableOperation<MainMenuUi.Command>();

        public MainMenuStateHandler(MainMenuUi mainMenuPrefab)
        {
            _menu = UnityEngine.Object.Instantiate(mainMenuPrefab);
        }

        public override void Dispose(in RootScope scope)
        {
            _uiPoll.Dispose();
            UnityEngine.Object.Destroy(_menu);
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
            var windowHandle = scope.Ui.AddStaticWindow(_menu.transform);

            var command = await _uiPoll.ExecuteAsync(ct);
            switch (command)
            {
                case MainMenuUi.Command.Play:
                    scope.Ui.RemoveStaticWindow(windowHandle);
                    return Transition.Replace(new LevelStateHandler());

                case MainMenuUi.Command.Exit:
                    scope.Ui.RemoveStaticWindow(windowHandle);
                    return Transition.Pop();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}