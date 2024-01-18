using System;
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

            ushort nextEntityId = 1; // Do not assign a zero id to anything

            for (var y = 0; y < dataGrid.Height; y++)
            for (var x = 0; x < dataGrid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref dataGrid.RefAt(point);
                ref var cell = ref state.Cells.RefAt(point);

                cell.WriteDefault();
                cell.Object.Id = cellData.ObjectType == BoardObjectType.Nothing ? default : nextEntityId++;
                cell.Tile.Id = cellData.TileType == BoardTileType.Nothing ? default : nextEntityId++;

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
                        cell.Tile.State.AsButton.WriteDefault();
                        break;
                    case BoardTileType.Spikes:
                        cell.Tile.State.AsSpikes.WriteDefault();
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

            ref var oldCell = ref state.Cells.RefAt(pos);
            if (!IsMovableObject(oldCell.Object.Type))
                return false;

            if (!CanLeaveTile(in oldCell.Tile))
                return false;

            ref var newCell = ref state.Cells.RefAt(newPos);
            if (!CanEnterTile(newCell.Tile, oldCell.Object.Type))
                return false;

            if (!CanReplaceObject(newCell.Object, oldCell.Object.Type) && !MoveObject(ref state, newPos, shift, timeline))
                return false;

            if (newCell.Object.Type != BoardObjectType.Nothing)
                CollectObject(ref state, ref newCell.Object, oldCell.Object.Type, timeline);

            Debug.Assert(newCell.Object.Type == BoardObjectType.Nothing);
            newCell.Object = oldCell.Object;
            oldCell.Object = default;

            if (newCell.Object.Type == BoardObjectType.Character)
            {
                state.Character.Position = newPos;
            }

            TileLeft(ref oldCell.Tile, in timeline);
            TileEntered(ref newCell.Tile, in timeline);

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

        private static void CollectObject(ref LevelState state, ref ObjectState obj, BoardObjectType byObjType, in TimeLine timeline)
        {
            switch (obj.Type)
            {
                case BoardObjectType.Coin:
                    timeline.AddDestroy(obj.Id);
                    obj.WriteDefault();
                    if (byObjType == BoardObjectType.Character)
                        state.Character.Coins++;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void TileLeft(ref TileState tile, in TimeLine timeline)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    tile.State.AsButton.IsPressed = false;
                    timeline.AddFlipFlop(tile.Id, false);
                    break;
            }
        }

        private static void TileEntered(ref TileState tile, in TimeLine timeline)
        {
            switch (tile.Type)
            {
                case BoardTileType.Button:
                    tile.State.AsButton.IsPressed = true;
                    timeline.AddFlipFlop(tile.Id, true);
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

                var isPressed = cell.Tile.State.AsButton.IsPressed;
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

                ref var spikesState = ref cell.Tile.State.AsSpikes;
                if (spikesState.IsActive != isActive)
                {
                    spikesState.IsActive = isActive;
                    timeline.AddFlipFlop(cell.Tile.Id, isActive);

                    if (cell.Object.Type == BoardObjectType.Character)
                    {
                        timeline.AddDestroy(cell.Object.Id);
                        state.Character.IsAlive = false;
                    }
                }
            }
        }
    }
}