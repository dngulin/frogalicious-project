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
        public static void SetupInitialState(ref SimState state, LevelData data)
        {
            state.TimeLine = new TimeLine(32);

            ref var indices = ref state.Indices;

            indices.Buttons = RefList.WithCapacity<BoardPoint>(8);
            indices.Spikes = RefList.WithCapacity<BoardPoint>(8);
            indices.Springs = RefList.WithCapacity<BoardPoint>(8);

            ref var level = ref state.Level;

            var dataGrid = data.AsBoardGrid();
            level.Cells.Array = RefList.WithDefaultItems<CellState>(data.Cells.Length);
            level.Cells.Width = dataGrid.Width;
            level.Cells.Height = dataGrid.Height;

            ushort nextEntityId = 1; // Do not assign a zero id to anything

            for (var y = 0; y < dataGrid.Height; y++)
            for (var x = 0; x < dataGrid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref dataGrid.RefAt(point);
                ref var cell = ref level.Cells.RefAt(point);

                cell.Object.Id = cellData.Object.Type == BoardObjectType.Nothing ? default : nextEntityId++;
                cell.Tile.Id = cellData.Tile.Type == BoardTileType.Nothing ? default : nextEntityId++;

                cell.Object.Type = cellData.Object.Type;
                if (cell.Object.Type == BoardObjectType.Character)
                {
                    level.Character.IsAlive = true;
                    level.Character.Position = point;
                }

                ref var tile = ref cell.Tile;
                tile.Type = cellData.Tile.Type;

                switch (tile.Type)
                {
                    case BoardTileType.Button:
                        tile.CfgHandle.AsColor = cellData.Tile.Color;
                        indices.Buttons.RefAdd() = point;
                        break;
                    case BoardTileType.Spikes:
                        tile.State.AsSpikes.IsActive = true;
                        tile.CfgHandle.AsColor = cellData.Tile.Color;
                        indices.Spikes.RefAdd() = point;
                        break;
                    case BoardTileType.Spring:
                        tile.CfgHandle.AsDirection = cellData.Tile.Direction;
                        indices.Springs.RefAdd() = point;
                        break;
                }
            }
        }

        public static void Simulate(ref SimState state, in InputState input)
        {
            Debug.Assert(state.TimeLine.Events.Count() == 0);

            if (input == InputState.None)
                return;

            if (!input.TryGetMoveDirection(out var direction))
                return;

            while (MakeMovements(ref state, direction))
            {
                UpdateButtons(ref state);
                state.TimeLine.Step++;
                CheckCharacterAlive(ref state);

                if (state.TimeLine.Step > 1000)
                {
                    Debug.LogError("Too many iterations");
                    break;
                }
            }
        }

        private static bool MakeMovements(ref SimState state, BoardDirection dir)
        {
            if (state.TimeLine.Step == 0 && MoveCharacter(ref state, dir))
                return true;

            return UpdateSprings(ref state);
        }

        private static bool MoveCharacter(ref SimState state, BoardDirection dir)
        {
            var shift = dir.ToBoardPoint();
            return MoveObject(ref state, state.Level.Character.Position, shift);
        }

        private static bool MoveObject(ref SimState state, BoardPoint pos, BoardPoint shift)
        {
            var newPos = pos + shift;
            if (!state.Level.Cells.HasPoint(newPos))
                return false;

            ref var oldCell = ref state.Level.Cells.RefAt(pos);
            if (!IsMovableObject(oldCell.Object.Type))
                return false;

            if (!CanLeaveTile(in oldCell.Tile))
                return false;

            ref var newCell = ref state.Level.Cells.RefAt(newPos);
            if (!CanEnterTile(newCell.Tile, oldCell.Object.Type))
                return false;

            if (!CanReplaceObject(newCell.Object, oldCell.Object.Type) && !MoveObject(ref state, newPos, shift))
                return false;

            if (newCell.Object.Type != BoardObjectType.Nothing)
                CollectObject(ref state, ref newCell.Object, oldCell.Object.Type);

            Debug.Assert(newCell.Object.Type == BoardObjectType.Nothing);
            newCell.Object = oldCell.Object;
            oldCell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Level.Character.Position = newPos;
            }

            TileLeft(ref oldCell.Tile, ref state.TimeLine);
            TileEntered(ref newCell.Tile, ref state.TimeLine);

            state.TimeLine.AddMove(newCell.Object.Id, pos, newPos);
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

        private static void CollectObject(ref SimState state, ref ObjectState obj, BoardObjectType byObjType)
        {
            switch (obj.Type)
            {
                case BoardObjectType.Coin:
                    state.TimeLine.AddDisappear(obj.Id);
                    obj = default;
                    if (byObjType == BoardObjectType.Character)
                        state.Level.Character.Coins++;
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

        private static void UpdateButtons(ref SimState state)
        {
            for (var i = 0; i < state.Indices.Buttons.Count(); i++)
            {
                var point = state.Indices.Buttons.RefAt(i);
                ref readonly var tile = ref state.Level.Cells.RefReadonlyAt(point).Tile;

                Debug.Assert(tile.Type == BoardTileType.Button);
                ref readonly var button = ref tile.State.AsButton;

                UpdateSpikes(ref state, !button.IsPressed, tile.CfgHandle.AsColor);
            }
        }

        private static void UpdateSpikes(ref SimState state, bool isActive, BoardColorGroup color)
        {
            for (var i = 0; i < state.Indices.Spikes.Count(); i++)
            {
                var point = state.Indices.Spikes.RefAt(i);
                ref var tile = ref state.Level.Cells.RefAt(point).Tile;

                Debug.Assert(tile.Type == BoardTileType.Spikes);
                ref var spikes = ref tile.State.AsSpikes;

                if (tile.CfgHandle.AsColor != color || spikes.IsActive == isActive)
                    continue;

                spikes.IsActive = isActive;
                state.TimeLine.AddFlipFlop(tile.Id, isActive);
            }
        }

        private static void CheckCharacterAlive(ref SimState state)
        {
            var charPos = state.Level.Character.Position;
            ref readonly var charCell = ref state.Level.Cells.RefReadonlyAt(charPos);
            if (charCell.Tile.Type == BoardTileType.Spikes && charCell.Tile.State.AsSpikes.IsActive)
            {
                Debug.Assert(charCell.Object.Type == BoardObjectType.Character);
                state.TimeLine.AddDisappear(charCell.Object.Id);
                state.Level.Character.IsAlive = false;
            }
        }

        private static bool UpdateSprings(ref SimState state)
        {
            var result = false;

            for (var i = 0; i < state.Indices.Springs.Count(); i++)
            {
                var point = state.Indices.Springs.RefAt(i);
                ref var cell = ref state.Level.Cells.RefAt(point);

                Debug.Assert(cell.Tile.Type == BoardTileType.Spring);

                if (cell.Object.Type == BoardObjectType.Nothing)
                    continue;

                if (state.TimeLine.IsEntityMovedThisStep(cell.Object.Id))
                    continue;

                var direction = cell.Tile.CfgHandle.AsDirection;
                var shift = direction.ToBoardPoint() + direction.ToBoardPoint();

                result |= ThrowObject(ref state, point, shift);
            }

            return result;
        }

        private static bool ThrowObject(ref SimState state, BoardPoint pos, BoardPoint shift)
        {
            var newPos = pos + shift;
            if (!state.Level.Cells.HasPoint(newPos))
                return false;

            ref var oldCell = ref state.Level.Cells.RefAt(pos);
            if (!IsMovableObject(oldCell.Object.Type))
                return false;

            ref var newCell = ref state.Level.Cells.RefAt(newPos);
            if (!CanEnterTile(newCell.Tile, oldCell.Object.Type))
                return false;

            if (!CanReplaceObject(newCell.Object, oldCell.Object.Type))
                return false;

            if (newCell.Object.Type != BoardObjectType.Nothing)
                CollectObject(ref state, ref newCell.Object, oldCell.Object.Type);

            Debug.Assert(newCell.Object.Type == BoardObjectType.Nothing);
            newCell.Object = oldCell.Object;
            oldCell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Level.Character.Position = newPos;
            }

            TileLeft(ref oldCell.Tile, ref state.TimeLine);
            TileEntered(ref newCell.Tile, ref state.TimeLine);

            state.TimeLine.AddMove(newCell.Object.Id, pos, newPos);
            return true;
        }
    }
}