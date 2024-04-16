using System;
using System.Threading;
using Frog.Meta.Level;
using Frog.StateTracker;
using UnityEngine;

namespace Frog.Meta.MainMenu
{
    public class MainMenuStateHandler : AsyncStateHandler<RootScope>
    {
        private readonly MainMenuUi _mainMenuPrefab;

        public MainMenuStateHandler(MainMenuUi mainMenuPrefab)
        {
            _mainMenuPrefab = mainMenuPrefab;
        }

        public override void Dispose() {}

        public override async Awaitable<Transition> Run(RootScope scope, CancellationToken ct)
        {
            var menu = UnityEngine.Object.Instantiate(_mainMenuPrefab);
            var window = await scope.Ui.OpenWindow(menu.transform);

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var command = await menu.WaitForCommand(ct);
                switch (command)
                {
                    case MainMenuUi.Command.Play:
                        await scope.Ui.CloseWindow(window);
                        return Transition.Replace(new LevelStateHandler());

                    case MainMenuUi.Command.Exit:
                        await scope.Ui.CloseWindow(window);
                        return Transition.Pop();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}