using Frog.Level.Primitives;

namespace Frog.LevelEditor.View
{
    internal struct DrawingBrushState
    {
        public DrawingLayer Layer;
        public BoardTileType TileType;
        public BoardObjectType ObjectType;

        public BoardColorGroup Color;
        public BoardDirection Direction;
    }

    internal enum DrawingLayer : uint
    {
        Tiles,
        Objects,
    }

    internal static class DrawingBrushStateExtensions
    {
        public static void Update(ref this DrawingBrushState brush, BoardTileType tile, BoardColorGroup color, BoardDirection direction)
        {
            brush = default;
            brush.Layer = DrawingLayer.Tiles;
            brush.TileType = tile;
            brush.Color = color;
            brush.Direction = direction;
        }

        public static void Update(ref this DrawingBrushState brush, BoardObjectType obj)
        {
            brush = default;
            brush.Layer = DrawingLayer.Objects;
            brush.ObjectType = obj;
        }

        public static int CalculateHash(in this DrawingBrushState brush)
        {
            return (brush.Layer, brush.TileType, brush.ObjectType, brush.Color, brush.Direction).GetHashCode();
        }
    }
}