using System.Collections.Generic;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.Simulation
{
    public class LevelSimulation
    {
        private readonly LevelData _data;

        public void Simulate(ref LevelState state, in InputState input, List<SimulationEvent> events)
        {
            Debug.Assert(events.Count == 0);

            if (input.TryGetMoveDirection(out var direction))
                return;

            var offset = direction.ToBoardPoint();
            var newPos = state.CharacterPosition + offset;

            ref readonly var cell = ref _data.CellAtPoint(newPos);
            if (cell.TileType != BoardTileType.Ground || cell.ObjectType != BoardObjectType.Nothing)
                return;

            events.Add(new SimulationEvent
            {
                Type = SimulationEventType.Move,
                Position = state.CharacterPosition,
                EndPosition = newPos,
                SenderId = 0,
                Step = 0,
            });
            state.CharacterPosition = newPos;
        }
    }
}