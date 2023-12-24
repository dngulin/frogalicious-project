using System;
using System.Collections.Generic;
using Frog.Level.Collections;
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
            var grid = _data.AsBoardGrid();

            for (var y = 0; y < grid.Height; y++)
            for (var x = 0; x < grid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref grid.RefAt(point);

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

            var grid = _data.AsBoardGrid();

            if (!grid.HasPoint(newPos))
                return;

            ref readonly var cell = ref grid.RefAt(newPos);
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