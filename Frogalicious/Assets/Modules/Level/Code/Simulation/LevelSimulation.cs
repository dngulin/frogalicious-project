using System;
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

        public LevelSimulation(LevelData data)
        {
            _data = data;
        }

        public LevelState CreateInitialState()
        {
            for (var y = 0; y < _data.Height; y++)
            for (var x = 0; x < _data.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref _data.CellAtPoint(point);

                if (cellData.ObjectType == BoardObjectType.Character)
                {
                    return new LevelState
                    {
                        CharacterPosition = point,
                    };
                }
            }

            throw new InvalidOperationException();
        }

        public void Simulate(ref LevelState state, in InputState input, List<TimeLineEvent> timeLine)
        {
            Debug.Assert(timeLine.Count == 0);

            if (!input.TryGetMoveDirection(out var direction))
                return;

            var offset = direction.ToBoardPoint();
            var newPos = state.CharacterPosition + offset;

            if (!_data.IsPointInBounds(newPos))
                return;

            ref readonly var cell = ref _data.CellAtPoint(newPos);
            if (cell.TileType != BoardTileType.Ground || cell.ObjectType == BoardObjectType.Obstacle)
                return;

            timeLine.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.Move,
                Position = state.CharacterPosition,
                EndPosition = newPos,
                SenderId = 0,
                Step = 0,
            });
            state.CharacterPosition = newPos;
        }
    }
}