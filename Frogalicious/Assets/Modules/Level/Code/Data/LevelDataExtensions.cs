using Frog.Level.Primitives;

namespace Frog.Level.Data
{
    public static class LevelDataExtensions
    {
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