using Frog.Level.Primitives;

namespace Frog.Level.Config
{
    public struct BoardCellConfig
    {
        public BoardTileHandle? Tile;
        public BoardObjectType? Object;
    }

    public struct BoardTileHandle
    {
        public BoardTileType Type;
        public ushort ConfigId;
    }

    public struct BoardObjectHandle
    {
        public BoardObjectType Type;
        public ushort ConfigId;
    }
}