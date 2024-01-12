using System;
using System.Collections.Generic;
using Frog.Level.Collections;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.Simulation
{
    public static class LevelSimulation
    {
        public static void Simulate(ref LevelState state, in InputState input, List<TimeLineEvent> timeline)
        {
            Debug.Assert(timeline.Count == 0);

            if (!input.TryGetMoveDirection(out var direction))
                return;

            if (!MoveCharacter(ref state, direction, timeline, 0))
            {
                return;
            }

            UpdateButtons(ref state, timeline, 1);
        }

        private static bool MoveCharacter(ref LevelState state, MoveDirection dir, List<TimeLineEvent> timeline, ushort step)
        {
            var shift = dir.ToBoardPoint();
            var newPos = state.Character.Position + shift;


            return MoveObject(ref state, state.Character.Position, shift, timeline, step);
        }

        private static bool MoveObject(ref LevelState state, BoardPoint pos, BoardPoint shift, List<TimeLineEvent> timeline, ushort step)
        {
            var newPos = pos + shift;
            if (!state.Cells.HasPoint(newPos))
                return false;

            ref var cell = ref state.Cells.RefMutAt(pos);
            if (!IsMovableObject(cell.ObjectType))
                return false;

            if (!CanLeaveTile(in state, in cell))
                return false;

            ref var newCell = ref state.Cells.RefMutAt(newPos);
            if (!CanEnterTile(in state, in newCell, cell.ObjectType))
                return false;

            if (newCell.ObjectType != BoardObjectType.Nothing && !MoveObject(ref state, newPos, shift, timeline, step))
                return false;

            newCell.ObjectType = cell.ObjectType;
            newCell.ObjectStateIdx = cell.ObjectStateIdx;

            cell.ObjectType = default;
            cell.ObjectStateIdx = default;

            if (newCell.ObjectType == BoardObjectType.Character)
            {
                state.Character.Position = newPos;
            }

            timeline.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.Move,
                Position = pos,
                EndPosition = newPos,
                Step = step,
            });
            return true;
        }

        private static bool IsMovableObject(BoardObjectType objType)
        {
            return objType switch {
                BoardObjectType.Character => true,
                BoardObjectType.Box => true,
                _ => false,
            };
        }

        private static bool CanEnterTile(in LevelState state, in CellState cell, BoardObjectType byObj)
        {
            return cell.TileType switch {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => byObj == BoardObjectType.Character || !state.Spikes[cell.ObjectStateIdx].IsActive,
                _ => false,
            };
        }

        private static bool CanLeaveTile(in LevelState state, in CellState cell)
        {
            return cell.TileType switch {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => !state.Spikes[cell.ObjectStateIdx].IsActive,
                _ => false,
            };
        }


        private static void UpdateButtons(ref LevelState state, List<TimeLineEvent> timeline, ushort step)
        {

        }
    }
}