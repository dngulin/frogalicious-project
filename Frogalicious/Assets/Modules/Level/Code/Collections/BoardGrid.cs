using System;

namespace Frog.Level.Collections
{
    [Serializable]
    public readonly struct BoardGrid<T>
    {
        public readonly T[] Cells;
        public readonly ushort Width;
        public readonly ushort Height;

        public BoardGrid(T[] cells, ushort width, ushort height)
        {
            Cells = cells;
            Width = width;
            Height = height;
        }
    }
}