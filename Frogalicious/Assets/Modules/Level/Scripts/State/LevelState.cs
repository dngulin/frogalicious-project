using System.Runtime.InteropServices;
using Frog.Collections;
using Frog.Level.Primitives;

namespace Frog.Level.State
{
    [NoCopy]
    public struct LevelState
    {
        public CellsState Cells;
        public CharacterState Character;
        public bool IsCompleted;
    }

    [NoCopy]
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
        public TileConfigHandle CfgHandle;
        public ushort Id;
        public ExtendedTileState State;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ExtendedTileState
    {
        [FieldOffset(0)] public ButtonState AsButton;
        [FieldOffset(0)] public SpikesState AsSpikes;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct TileConfigHandle
    {
        [FieldOffset(0)] public BoardColorGroup AsColor;
        [FieldOffset(0)] public BoardDirection AsDirection;
    }

    public struct ButtonState
    {
        public bool IsPressed;
    }

    public struct SpikesState
    {
        public bool IsActive;
    }
}