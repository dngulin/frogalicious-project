using System;
using Frog.Collections;
using Frog.Level.Data;
using Frog.Level.Simulation;
using Frog.Level.View;

namespace Frog.Level
{
    public partial class LevelController : IDisposable
    {
        private SimState _state;

        private readonly LevelView _view;

        private LevelController(LevelView view, LevelData data)
        {
            _state.TimeLine = new TimeLine(32);
            LevelSimulation.SetupInitialState(ref _state.Level, data);
            view.CreateInitialObjects(in _state.Level);
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

            LevelSimulation.Simulate(ref _state, InputStateProvider.Poll());

            if (_state.TimeLine.Events.Count() == 0)
                return;

            _view.StartPlayingTimeline(_state.TimeLine.Events);
            _state.TimeLine.Reset();
        }
    }
}