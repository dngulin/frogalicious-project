using System;
using Frog.Collections;
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
            state.Cells.Array = RefList<CellState>.CreateWithDefaultItems(data.Cells.Length);
            state.Cells.Width = dataGrid.Width;
            state.Cells.Height = dataGrid.Height;

            ushort nextEntityId = 1; // Do not assign a zero id to anything

            for (var y = 0; y < dataGrid.Height; y++)
            for (var x = 0; x < dataGrid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref dataGrid.RefAt(point);
                ref var cell = ref state.Cells.RefAt(point);

                cell.Object.Id = cellData.Object.Type == BoardObjectType.Nothing ? default : nextEntityId++;
                cell.Tile.Id = cellData.Tile.Type == BoardTileType.Nothing ? default : nextEntityId++;

                cell.Object.Type = cellData.Object.Type;
                if (cell.Object.Type == BoardObjectType.Character)
                {
                    state.Character.IsAlive = true;
                    state.Character.Position = point;
                }

                cell.Tile.Type = cellData.Tile.Type;
                switch (cellData.Tile.Type)
                {
                    case BoardTileType.Button:
                        ref var button = ref cell.Tile.State.AsButton;
                        button.Color = cellData.Tile.Color;
                        break;
                    case BoardTileType.Spikes:
                        ref var spikes = ref cell.Tile.State.AsSpikes;
                        spikes.IsActive = true;
                        spikes.Color = cellData.Tile.Color;
                        break;
                    case BoardTileType.Spring:
                        ref var spring = ref cell.Tile.State.AsSpring;
                        spring.Direction = cellData.Tile.Direction;
                        break;
                }
            }
        }

        public static void Simulate(ref LevelState state, in InputState input, ref TimeLine timeline)
        {
            Debug.Assert(timeline.Events.Count() == 0);

            if (input == InputState.None)
                return;

            if (!input.TryGetMoveDirection(out var direction))
                return;

            while (MakeMovements(ref state, direction, ref timeline))
            {
                UpdateButtons(ref state, ref timeline);
                timeline.Step++;
                CheckCharacterAlive(ref state, timeline);

                if (timeline.Step > 1000)
                {
                    Debug.LogError("Too many iterations");
                    break;
                }
            }
        }

        private static bool MakeMovements(ref LevelState state, BoardDirection dir, ref TimeLine timeline)
        {
            if (timeline.Step == 0 && MoveCharacter(ref state, dir, ref timeline))
                return true;

            return UpdateSprings(ref state, ref timeline);
        }

        private static bool MoveCharacter(ref LevelState state, BoardDirection dir, ref TimeLine timeline)
        {
            var shift = dir.ToBoardPoint();
            return MoveObject(ref state, state.Character.Position, shift, ref timeline);
        }

        private static bool MoveObject(ref LevelState state, BoardPoint pos, BoardPoint shift, ref TimeLine timeline)
        {
            var newPos = pos + shift;
            if (!state.Cells.HasPoint(newPos))
                return false;

            ref var oldCell = ref state.Cells.RefAt(pos);
            if (!IsMovableObject(oldCell.Object.Type))
                return false;

            if (!CanLeaveTile(in oldCell.Tile))
                return false;

            ref var newCell = ref state.Cells.RefAt(newPos);
            if (!CanEnterTile(newCell.Tile, oldCell.Object.Type))
                return false;

            if (!CanReplaceObject(newCell.Object, oldCell.Object.Type) && !MoveObject(ref state, newPos, shift, ref timeline))
                return false;

            if (newCell.Object.Type != BoardObjectType.Nothing)
                CollectObject(ref state, ref newCell.Object, oldCell.Object.Type, ref timeline);

            Debug.Assert(newCell.Object.Type == BoardObjectType.Nothing);
            newCell.Object = oldCell.Object;
            oldCell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Character.Position = newPos;
            }

            TileLeft(ref oldCell.Tile, ref timeline);
            TileEntered(ref newCell.Tile, ref timeline);

            timeline.AddMove(newCell.Object.Id, pos, newPos);
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

        private static bool CanEnterTile(in TileState tile, BoardObjectType byObj)
        {
            return tile.Type switch
            {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => byObj == BoardObjectType.Character || !tile.State.AsSpikes.IsActive,
                BoardTileType.Spring => true,
                _ => false,
            };
        }

        private static bool CanLeaveTile(in TileState tile)
        {
            return tile.Type switch
            {
                BoardTileType.Ground => true,
                BoardTileType.Button => true,
                BoardTileType.Spikes => !tile.State.AsSpikes.IsActive,
                BoardTileType.Spring => true,
                _ => false,
            };
        }

        private static bool CanReplaceObject(in ObjectState obj, BoardObjectType byObjType)
        {
            if (obj.Type == BoardObjectType.Nothing)
                return true;

            if (byObjType != BoardObjectType.Character)
                return false;

            return obj.Type == BoardObjectType.Coin;
        }

        private static void CollectObject(ref LevelState state, ref ObjectState obj, BoardObjectType byObjType, ref TimeLine timeline)
        {
            switch (obj.Type)
            {
                case BoardObjectType.Coin:
                    timeline.AddDisappear(obj.Id);
                    obj = default;
                    if (byObjType == BoardObjectType.Character)
                        state.Character.Coins++;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void TileLeft(ref TileState tile, ref TimeLine timeline)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    tile.State.AsButton.IsPressed = false;
                    timeline.AddFlipFlop(tile.Id, false);
                    break;
            }
        }

        private static void TileEntered(ref TileState tile, ref TimeLine timeline)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    tile.State.AsButton.IsPressed = true;
                    timeline.AddFlipFlop(tile.Id, true);
                    break;
            }
        }

        private static void UpdateButtons(ref LevelState state, ref TimeLine timeline)
        {
            for (var y = 0; y < state.Cells.Height; y++)
            for (var x = 0; x < state.Cells.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cell = ref state.Cells.RefReadonlyAt(point);
                if (cell.Tile.Type != BoardTileType.Button)
                    continue;

                ref readonly var button = ref cell.Tile.State.AsButton;

                UpdateSpikes(ref state, !button.IsPressed, button.Color, ref timeline);
            }
        }

        private static void UpdateSpikes(ref LevelState state, bool isActive, BoardColorGroup color, ref TimeLine timeline)
        {
            for (var y = 0; y < state.Cells.Height; y++)
            for (var x = 0; x < state.Cells.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref var cell = ref state.Cells.RefAt(point);
                if (cell.Tile.Type != BoardTileType.Spikes)
                    continue;

                ref var spikes = ref cell.Tile.State.AsSpikes;
                if (spikes.Color != color)
                    continue;

                if (spikes.IsActive != isActive)
                {
                    spikes.IsActive = isActive;
                    timeline.AddFlipFlop(cell.Tile.Id, isActive);
                }
            }
        }

        private static void CheckCharacterAlive(ref LevelState state, TimeLine timeline)
        {
            ref readonly var charCell = ref state.Cells.RefReadonlyAt(state.Character.Position);
            if (charCell.Tile.Type == BoardTileType.Spikes && charCell.Tile.State.AsSpikes.IsActive)
            {
                Debug.Assert(charCell.Object.Type == BoardObjectType.Character);
                timeline.AddDisappear(charCell.Object.Id);
                state.Character.IsAlive = false;
            }
        }

        private static bool UpdateSprings(ref LevelState state, ref TimeLine timeline)
        {
            var result = false;

            for (var y = 0; y < state.Cells.Height; y++)
            for (var x = 0; x < state.Cells.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref var cell = ref state.Cells.RefAt(point);
                if (cell.Tile.Type != BoardTileType.Spring || cell.Object.Type == BoardObjectType.Nothing)
                    continue;

                if (timeline.IsEntityMovedThisStep(cell.Object.Id))
                    continue;

                var direction = cell.Tile.State.AsSpring.Direction;
                var shift = direction.ToBoardPoint() + direction.ToBoardPoint();

                result |= ThrowObject(ref state, point, shift, ref timeline);
            }

            return result;
        }

        private static bool ThrowObject(ref LevelState state, BoardPoint pos, BoardPoint shift, ref TimeLine timeline)
        {
            var newPos = pos + shift;
            if (!state.Cells.HasPoint(newPos))
                return false;

            ref var oldCell = ref state.Cells.RefAt(pos);
            if (!IsMovableObject(oldCell.Object.Type))
                return false;

            ref var newCell = ref state.Cells.RefAt(newPos);
            if (!CanEnterTile(newCell.Tile, oldCell.Object.Type))
                return false;

            if (!CanReplaceObject(newCell.Object, oldCell.Object.Type))
                return false;

            if (newCell.Object.Type != BoardObjectType.Nothing)
                CollectObject(ref state, ref newCell.Object, oldCell.Object.Type, ref timeline);

            Debug.Assert(newCell.Object.Type == BoardObjectType.Nothing);
            newCell.Object = oldCell.Object;
            oldCell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Character.Position = newPos;
            }

            TileLeft(ref oldCell.Tile, ref timeline);
            TileEntered(ref newCell.Tile, ref timeline);

            timeline.AddMove(newCell.Object.Id, pos, newPos);
            return true;
        }
    }
}