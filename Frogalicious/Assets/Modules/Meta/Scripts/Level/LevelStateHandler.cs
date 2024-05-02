using System.Threading;
using Frog.Collections;
using Frog.Core;
using Frog.Level;
using Frog.Level.Data;
using Frog.Level.Simulation;
using Frog.Level.Ui;
using Frog.Level.View;
using Frog.StateTracker;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Frog.Meta.Level
{
    public class LevelStateHandler : AsyncStateHandler<RootScope>
    {
        private readonly LevelView _view;
        private readonly LevelPanelUi _ui;

        private SimState _state;

        private readonly AwaitableOperation _gameplay = new AwaitableOperation();

        public LevelStateHandler(in RootScope scope, LevelData data, LevelViewConfig viewConfig, LevelPanelUi uiPrefab)
        {
            _view = new LevelView(viewConfig, data, scope.Camera);
            _ui = Object.Instantiate(uiPrefab);

            LevelSimulation.SetupInitialState(ref _state, data);
            _view.CreateInitialObjects(in _state.Level);
        }

        public override void Dispose(in RootScope scope)
        {
            _view.Dispose();
            _ui.DestroyGameObject();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_ui.Poll().TryGetValue(out _))
            {
                _gameplay.EndAssertive();
                return;
            }

            _view.Tick(dt);
            if (_view.IsPlayingTimeline)
                return;

            LevelSimulation.Simulate(ref _state, InputStateProvider.Poll());

            if (_state.TimeLine.Events.Count() == 0)
                return;

            _view.StartPlayingTimeline(_state.TimeLine.Events);
            _state.TimeLine.Reset();
        }

        public override async Awaitable<Transition> ExecuteAsync(RootScope scope, CancellationToken ct)
        {
            var menuHandle = scope.Ui.ShowFullscreenWindow(_ui.transform);
            await _gameplay.ExecuteAsync(ct);
            scope.Ui.HideFullscreenWindow(menuHandle);

            return Transition.Pop();
        }
    }
}