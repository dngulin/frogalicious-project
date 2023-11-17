using Frog.Level.Primitives;

namespace Frog.Level.Data
{
    public static class LevelDataExtensions
    {
        public static bool IsPointInBounds(this LevelData data, in BoardPoint point)
        {
            if (point.X < 0 || point.Y < 0)
                return false;

            if (point.X >= data.Width || point.Y >= data.Height)
                return false;

            return true;
        }

        public static ref readonly CellData CellAtPoint(this LevelData data, in BoardPoint point)
        {
            return ref data.Cells[point.Y * data.Width + point.X];
        }

        public static ref CellData CellAtPointMut(this LevelData data, in BoardPoint point)
        {
            return ref data.Cells[point.Y * data.Width + point.X];
        }
    }
}