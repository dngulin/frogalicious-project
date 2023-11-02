using System;

namespace Frog.Level.Primitives
{
    [Flags]
    public enum BoardLayers : ushort
    {
        Tiles = 1 << 0,
        Objects = 1 << 1,
    }
}
