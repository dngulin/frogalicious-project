using System.Runtime.InteropServices;
using Frog.Level.Primitives;
using Frog.RefList;

namespace Frog.Level.State
{
    public struct LevelState
    {
        public CellsState Cells;
        public CharacterState Character;
    }

    public struct CellsState {
        public RefList<CellState> Array;
        public ushort Width;
        public ushort Height;
    }

    public struct CellState
    {
        public ObjectState Object;
        public TileState Tile;
    }

    public struct CharacterState
    {
        public BoardPoint Position;
        public bool IsAlive;
        public ushort Coins;
    }

    public struct ObjectState
    {
        public BoardObjectType Type;
        public ushort Id;
    }

    public struct TileState
    {
        public BoardTileType Type;
        public ushort Id;
        public ExtendedTileState State;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ExtendedTileState
    {
        [FieldOffset(0)] public ButtonState AsButton;
        [FieldOffset(0)] public SpikesState AsSpikes;
        [FieldOffset(0)] public SpringState AsSpring;
    }

    public struct ButtonState
    {
        public BoardColorGroup Color;
        public bool IsPressed;
    }

    public struct SpikesState
    {
        public BoardColorGroup Color;
        public bool IsActive;
    }

    public struct SpringState
    {
        public BoardDirection Direction;
    }
}