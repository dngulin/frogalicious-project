using System;
using Frog.Core;
using Frog.Meta.MainMenu.View;

namespace Frog.Meta.MainMenu
{
    public class MainMenuView : IDisposable
    {
        private MapView _map;

        public MainMenuView(MapView map)
        {
            _map = map;
        }

        public void Dispose() => _map.DestroyGameObject();

        public int? PollLevelClick() => _map.PollLevelClick();
    }
}