using System.Collections.Generic;
using Frog.Level.Simulation;
using Frog.Level.State;
using Frog.Level.View;

namespace Frog.Level
{
    public class LevelController
    {
        private LevelState _state;

        private readonly LevelSimulation _simulation;
        private readonly LevelView _view;

        private readonly List<SimulationEvent> _simEvents;

        public void Tick(float dt)
        {
            _view.Tick(dt);
            if (_view.IsBusy)
                return;

            var input = InputStateProvider.Poll();
            _simulation.Simulate(ref _state, input, _simEvents);

            if (_simEvents.Count == 0)
                return;

            _view.DisplayEvents(_simEvents);
            _simEvents.Clear();
        }
    }
}