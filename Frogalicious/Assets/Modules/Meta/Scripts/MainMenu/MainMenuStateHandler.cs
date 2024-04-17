using System;
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
        private readonly MainMenuUi _mainMenuPrefab;

        private bool _menuOpened;
        private UiWindowHandle _windowHandle;
        private MainMenuUi _menu;
        private readonly AwaitableProcess<MainMenuUi.Command> _menuAwaiter = new AwaitableProcess<MainMenuUi.Command>();

        public MainMenuStateHandler(MainMenuUi mainMenuPrefab)
        {
            _mainMenuPrefab = mainMenuPrefab;
        }

        public override void Dispose(in RootScope scope)
        {
            Debug.Assert(_menuOpened);
            Debug.Assert(_menu != null);

            _menuAwaiter.Dispose();
            UnityEngine.Object.Destroy(_menu);
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_menuOpened && _menu.Poll().TryGetValue(out var command))
            {
                _menuAwaiter.EndWithAssert(command);
            }
        }

        public override async Awaitable<Transition> Run(RootScope scope, CancellationToken ct)
        {
            if (!_menuOpened)
            {
                await OpenMenuAsync(scope);
                _menuOpened = true;
            }

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var command = await _menuAwaiter.Begin(ct);
                switch (command)
                {
                    case MainMenuUi.Command.Play:
                        await scope.Ui.CloseWindow(_windowHandle);
                        return Transition.Replace(new LevelStateHandler());

                    case MainMenuUi.Command.Exit:
                        await scope.Ui.CloseWindow(_windowHandle);
                        return Transition.Pop();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Awaitable OpenMenuAsync(RootScope scope)
        {
            Debug.Assert(_menu == null);

            _menu = UnityEngine.Object.Instantiate(_mainMenuPrefab);
            _windowHandle = await scope.Ui.OpenWindow(_menu.transform);
        }
    }
}