using Frog.Level.Collections;
using Frog.Level.Data;
using Frog.Level.Primitives;
using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.Simulation
{
    public static class LevelSimulation
    {
        private const ushort CharacterId = 1000;

        public static void SetupInitialState(ref LevelState state, LevelData data)
        {
            var dataGrid = data.AsBoardGrid();
            state.Cells.Width = dataGrid.Width;
            state.Cells.Height = dataGrid.Height;

            state.EntityCount = 0;

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
                    cell.Object.EntityId = CharacterId;
                    state.Character.WriteDefault();
                    state.Character.Position = point;
                }

                cell.Tile.Type = cellData.TileType;
                switch (cellData.TileType)
                {
                    case BoardTileType.Button:
                        cell.Tile.EntityId = state.EntityCount++;
                        state.Entities.RefAt(cell.Tile.EntityId).AsButton.WriteDefault();
                        break;
                    case BoardTileType.Spikes:
                        cell.Tile.EntityId = state.EntityCount++;
                        state.Entities.RefAt(cell.Tile.EntityId).AsSpikes.WriteDefault();
                        break;
                }
            }
        }

        public static void Simulate(ref LevelState state, in InputState input, ref TimeLine timeline)
        {
            Debug.Assert(timeline.IsEmpty);

            if (!input.TryGetMoveDirection(out var direction))
                return;

            timeline.Step = 0;
            if (!MoveCharacter(ref state, direction, in timeline))
                return;

            timeline.Step++;
            CheckButtons(ref state, in timeline);
        }

        private static bool MoveCharacter(ref LevelState state, MoveDirection dir, in TimeLine timeline)
        {
            var shift = dir.ToBoardPoint();
            return MoveObject(ref state, state.Character.Position, shift, in timeline);
        }

        private static bool MoveObject(ref LevelState state, BoardPoint pos, BoardPoint shift, in TimeLine timeline)
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

            if (newCell.Object.Type != BoardObjectType.Nothing && !MoveObject(ref state, newPos, shift, in timeline))
                return false;

            newCell.Object = cell.Object;
            cell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Character.Position = newPos;
            }

            TileLeft(ref state, in cell.Tile, in timeline);
            TileEntered(ref state, in newCell.Tile, in timeline);

            timeline.AddMove(cell.Object.EntityId, pos, newPos);
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
                                        !state.Entities.RefReadonlyAt(tile.EntityId).AsSpikes.IsActive,
                _ => false,
            };
        }

        private static bool CanLeaveTile(in LevelState state, in TileState tile)
        {
            return tile.Type switch
            {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => !state.Entities.RefReadonlyAt(tile.EntityId).AsSpikes.IsActive,
                _ => false,
            };
        }

        private static void TileLeft(ref LevelState state, in TileState tile, in TimeLine timeline)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    state.Entities.RefAt(tile.EntityId).AsButton.IsPressed = false;
                    timeline.AddFlipFlop(tile.EntityId, false);
                    break;
            }
        }

        private static void TileEntered(ref LevelState state, in TileState tile, in TimeLine timeline)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    state.Entities.RefAt(tile.EntityId).AsButton.IsPressed = true;
                    timeline.AddFlipFlop(tile.EntityId, true);
                    break;
            }
        }

        private static void CheckButtons(ref LevelState state, in TimeLine timeline)
        {
            for (var y = 0; y < state.Cells.Height; y++)
            for (var x = 0; x < state.Cells.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cell = ref state.Cells.RefReadonlyAt(point);
                if (cell.Tile.Type != BoardTileType.Button)
                    continue;

                var isPressed = state.Entities.RefReadonlyAt(cell.Tile.EntityId).AsButton.IsPressed;
                UpdateSpikes(ref state, !isPressed, in timeline);
            }
        }

        private static void UpdateSpikes(ref LevelState state, bool isActive, in TimeLine timeline)
        {
            for (var y = 0; y < state.Cells.Height; y++)
            for (var x = 0; x < state.Cells.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref var cell = ref state.Cells.RefAt(point);
                if (cell.Tile.Type != BoardTileType.Spikes)
                    continue;

                ref var spikesState = ref state.Entities.RefAt(cell.Tile.EntityId).AsSpikes;
                if (spikesState.IsActive != isActive)
                {
                    spikesState.IsActive = isActive;
                    timeline.AddFlipFlop(cell.Tile.EntityId, isActive);
                }
            }
        }
    }
}