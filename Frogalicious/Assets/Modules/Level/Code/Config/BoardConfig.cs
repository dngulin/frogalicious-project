using Frog.Level.Primitives;

namespace Frog.Level.Config
{
    public struct BoardConfig
    {
        public ushort Width;
        public ushort Height;
        public BoardCellConfig[] Cells;

        public BoardPoint EntryPoint;
    }
}