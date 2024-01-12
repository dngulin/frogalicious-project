using System;
using System.Collections.Generic;
using Frog.Level.Collections;
using Frog.Level.Data;
using Frog.Level.Primitives;

namespace Frog.Level.State
{
    public struct LevelState
    {
        public readonly BoardGrid<CellState> Cells;

        public CharacterState Character;
        public ButtonState[] Buttons;
        public SpikesState[] Spikes;

        public LevelState(LevelData data)
        {
            Character = new CharacterState { IsAlive = true };

            var cells = new CellState[data.Cells.Length];
            Cells = new BoardGrid<CellState>(cells, data.Width, data.Height);

            var buttons = new List<ButtonState>();
            var spikes = new List<SpikesState>();

            var dataGrid = data.AsBoardGrid();

            for (var y = 0; y < dataGrid.Height; y++)
            for (var x = 0; x < dataGrid.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cellData = ref dataGrid.RefAt(point);

                ushort tileStateIdx = default;

                switch (cellData.TileType)
                {
                    case BoardTileType.Button:
                        tileStateIdx = (ushort)buttons.Count;
                        buttons.Add(default);
                        break;
                    case BoardTileType.Spikes:
                        tileStateIdx = (ushort)spikes.Count;
                        spikes.Add(new SpikesState {IsActive = true});
                        break;
                }

                Cells.RefMutAt(point) = new CellState
                {
                    ObjectType = cellData.ObjectType,
                    ObjectStateIdx = default,
                    TileType = cellData.TileType,
                    TileStateIdx = tileStateIdx,
                };

                if (cellData.ObjectType == BoardObjectType.Character)
                {
                    Character.Position = point;
                }
            }

            Buttons = buttons.ToArray();
            Spikes = spikes.ToArray();
        }
    }
}

public struct CellState
{
    public BoardObjectType ObjectType;
    public ushort ObjectStateIdx;

    public BoardTileType TileType;
    public ushort TileStateIdx;
}

public struct CharacterState
{
    public BoardPoint Position;
    public bool IsAlive;
}

public struct ButtonState
{
    public bool IsPressed;
}

public struct SpikesState
{
    public bool IsActive;
}