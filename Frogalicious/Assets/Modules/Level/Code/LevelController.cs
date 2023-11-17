using System;
using System.Collections.Generic;
using Frog.Level.Simulation;
using Frog.Level.State;
using Frog.Level.View;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Frog.Level
{
    public partial class LevelController : IDisposable
    {
        private LevelState _state;

        private readonly LevelSimulation _simulation;
        private readonly LevelView _view;

        private readonly List<SimulationEvent> _simEvents = new List<SimulationEvent>();

        private LevelController(LevelState state, LevelSimulation simulation, LevelView view)
        {
            _state = state;
            _simulation = simulation;
            _view = view;
        }

        public void Dispose()
        {
            _view.Dispose();
        }

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