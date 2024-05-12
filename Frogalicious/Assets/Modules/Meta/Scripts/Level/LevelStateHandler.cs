using System.Threading;
using Frog.Collections;
using Frog.Core;
using Frog.Core.Ui;
using Frog.Level;
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
        private LevelResources _res;

        private readonly LevelView _view;
        private readonly LevelPanelUi _panel;

        private SimState _state;

        private readonly AwaitableOperation _gameplay = new AwaitableOperation();

        public LevelStateHandler(RootScope scope, LevelResources res)
        {
            _res = res;
            _view = new LevelView(res.ViewConfig, res.Data, scope.Camera);
            _panel = Object.Instantiate(res.PanelPrefab, scope.GameObjectStash);

            LevelSimulation.SetupInitialState(ref _state, res.Data);
            _view.CreateInitialObjects(in _state.Level);
        }

        public override void Dispose(in RootScope scope)
        {
            _view.Dispose();
            _panel.DestroyGameObject();
            _res.Dispose();
        }

        public override void Tick(in RootScope scope, float dt)
        {
            if (_panel.Poll().TryGetValue(out _))
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
            await using (await scope.Ui.AnimatedUi(_panel, ct))
            {
                await _gameplay.ExecuteAsync(ct);
                return Transition.Pop();
            }
        }
    }
}