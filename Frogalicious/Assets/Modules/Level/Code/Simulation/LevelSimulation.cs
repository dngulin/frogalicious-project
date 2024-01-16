using System;
using System.Collections.Generic;
using Frog.Level.Collections;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.Simulation
{
    public static class LevelSimulation
    {
        public static void SetupInitialState(ref LevelState state, LevelData data)
        {
            var dataGrid = data.AsBoardGrid();
            state.Cells.Width = dataGrid.Width;
            state.Cells.Height = dataGrid.Height;

            state.ButtonsCount = 0;
            state.SpikesCount = 0;

            for (var y = 0; y < dataGrid.Height; y++)
            for (var x = 0; x < dataGrid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref dataGrid.RefAt(point);
                ref var cell = ref state.Cells.RefAt(point);

                cell.WriteDefault();

                cell.Object.Type = cellData.ObjectType;
                if (cell.Object.Type == BoardObjectType.Character)
                {
                    state.Character.WriteDefault();
                    state.Character.Position = point;
                }

                cell.Tile.Type = cellData.TileType;
                switch (cellData.TileType)
                {
                    case BoardTileType.Button:
                        cell.Tile.StateIdx = state.ButtonsCount++;
                        state.Buttons.RefAt(cell.Tile.StateIdx).WriteDefault();
                        break;
                    case BoardTileType.Spikes:
                        cell.Tile.StateIdx = state.SpikesCount++;
                        state.Spikes.RefAt(cell.Tile.StateIdx).WriteDefault();
                        break;
                }
            }
        }

        public static void Simulate(ref LevelState state, in InputState input, List<TimeLineEvent> timeline)
        {
            Debug.Assert(timeline.Count == 0);

            if (!input.TryGetMoveDirection(out var direction))
                return;

            if (!MoveCharacter(ref state, direction, timeline, 0))
                return;

            UpdateButtons(ref state, timeline, 1);
        }

        private static bool MoveCharacter(ref LevelState state, MoveDirection dir, List<TimeLineEvent> timeline, ushort step)
        {
            var shift = dir.ToBoardPoint();
            return MoveObject(ref state, state.Character.Position, shift, timeline, step);
        }

        private static bool MoveObject(ref LevelState state, BoardPoint pos, BoardPoint shift,
            List<TimeLineEvent> timeline, ushort step)
        {
            var newPos = pos + shift;
            if (!state.Cells.HasPoint(newPos))
                return false;

            ref var cell = ref state.Cells.RefAt(pos);
            if (!IsMovableObject(cell.Object.Type))
                return false;

            if (!CanLeaveTile(in state, in cell.Tile))
                return false;

            ref var newCell = ref state.Cells.RefAt(newPos);
            if (!CanEnterTile(in state, in newCell.Tile, cell.Object.Type))
                return false;

            if (newCell.Object.Type != BoardObjectType.Nothing && !MoveObject(ref state, newPos, shift, timeline, step))
                return false;

            newCell.Object = cell.Object;
            cell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Character.Position = newPos;
            }

            TileLeft(ref state, in cell.Tile, timeline, step);
            TileEntered(ref state, in newCell.Tile, timeline, step);

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
            return objType switch
            {
                BoardObjectType.Character => true,
                BoardObjectType.Box => true,
                _ => false,
            };
        }

        private static bool CanEnterTile(in LevelState state, in TileState tile, BoardObjectType byObj)
        {
            return tile.Type switch
            {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => byObj == BoardObjectType.Character ||
                                        !state.Spikes.RefReadonlyAt(tile.StateIdx).IsActive,
                _ => false,
            };
        }

        private static bool CanLeaveTile(in LevelState state, in TileState tile)
        {
            return tile.Type switch
            {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => !state.Spikes.RefReadonlyAt(tile.StateIdx).IsActive,
                _ => false,
            };
        }

        private static void TileLeft(ref LevelState state, in TileState tile, List<TimeLineEvent> timeline, ushort step)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    state.Buttons.RefAt(tile.StateIdx).IsPressed = false;
                    timeline.Add(new TimeLineEvent()); // TODO: FlipFlop event
                    break;
            }
        }

        private static void TileEntered(ref LevelState state, in TileState tile, List<TimeLineEvent> timeline, ushort step)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    state.Buttons.RefAt(tile.StateIdx).IsPressed = true;
                    timeline.Add(new TimeLineEvent()); // TODO: FlipFlop event
                    break;
                case BoardTileType.Spikes:
                    if (state.Spikes.RefReadonlyAt(tile.StateIdx).IsActive)
                    {
                        // TODO; Kill character
                    }
                    break;
            }
        }

        private static void UpdateButtons(ref LevelState state, List<TimeLineEvent> timeline, ushort step)
        {
            for (var buttonIdx = 0; buttonIdx < state.ButtonsCount; buttonIdx++)
            {
                var isPressed = state.Buttons.RefReadonlyAt(buttonIdx).IsPressed;

                for (var spikesIdx = 0; spikesIdx < state.SpikesCount; spikesIdx++)
                {
                    ref var spikes = ref state.Spikes.RefAt(spikesIdx);
                    var shouldBeActive = !isPressed;

                    if (spikes.IsActive != shouldBeActive)
                    {
                        spikes.IsActive = shouldBeActive;
                        timeline.Add(new TimeLineEvent()); // TODO: FlipFlop event
                    }
                }
            }
        }
    }
}