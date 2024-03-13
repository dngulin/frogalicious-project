using System;
using Frog.Level.Data;
using Frog.Level.Primitives;

namespace Frog.LevelEditor.Data
{
    public static class BoardTileHandleExtensions
    {
        public static void SetVariant(this ref BoardTileHandle handle, BoardColorGroup color, BoardDirection dir)
        {
            switch (handle.Type)
            {
                case BoardTileType.Nothing:
                case BoardTileType.Ground:
                    break;

                case BoardTileType.Button:
                case BoardTileType.Spikes:
                    handle.Variant = (ushort)color;
                    break;

                case BoardTileType.Spring:
                    handle.Variant = (ushort)dir;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}