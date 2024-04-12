using System;
using Frog.StateTracker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Meta.MainMenu
{
    public class MainMenuStateHandler : AsyncStateHandler<RootScope>
    {
        private readonly MainMenuUi _mainMenuPrefab;

        public MainMenuStateHandler(MainMenuUi mainMenuPrefab)
        {
            _mainMenuPrefab = mainMenuPrefab;
        }

        public override async Awaitable<Transition> Run(RootScope scope)
        {
            var menu = Object.Instantiate(_mainMenuPrefab);
            await scope.Ui.OpenWindow(menu.transform);

            while (true)
            {
                var command = await menu.WaitForCommand();
                switch (command)
                {
                    case MainMenuUi.Command.Play:
                        Debug.Log("Under construction!");
                        break;

                    case MainMenuUi.Command.Exit:
                        return Transition.Pop();

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}