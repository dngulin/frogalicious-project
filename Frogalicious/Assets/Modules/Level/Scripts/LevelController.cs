using System;
using Frog.Collections;
using Frog.Level.Data;
using Frog.Level.Simulation;
using Frog.Level.State;
using Frog.Level.View;

namespace Frog.Level
{
    public partial class LevelController : IDisposable
    {
        private LevelState _state;
        private TimeLine _timeLine = new TimeLine(32);

        private readonly LevelView _view;

        private LevelController(LevelView view, LevelData data)
        {
            LevelSimulation.SetupInitialState(ref _state, data);
            view.CreateInitialObjects(in _state);
            _view = view;
        }

        public void Dispose()
        {
            _view.Dispose();
        }

        public void Tick(float dt)
        {
            _view.Tick(dt);
            if (_view.IsPlayingTimeline)
                return;

            LevelSimulation.Simulate(ref _state, InputStateProvider.Poll(), ref _timeLine);

            if (_timeLine.Events.Count() == 0)
                return;

            _view.StartPlayingTimeline(in _timeLine.Events);
            _timeLine.Reset();
        }
    }
}