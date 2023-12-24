using System;
using Frog.Level.Primitives;

namespace Frog.Level.Collections
{
    public static class BoardGridExtensions
    {
        public static bool HasPoint<T>(this in BoardGrid<T> grid, in BoardPoint point)
        {
            if (point.X < 0 || point.Y < 0)
                return false;

            if (point.X >= grid.Width || point.Y >= grid.Height)
                return false;

            return true;
        }

        public static ref readonly T RefAt<T>(this in BoardGrid<T> data, in BoardPoint point)
        {
            if (!data.HasPoint(point))
                throw new IndexOutOfRangeException();

            return ref data.Cells[point.Y * data.Width + point.X];
        }

        public static ref T RefAtMut<T>(this in BoardGrid<T> grid, in BoardPoint point)
        {
            if (!grid.HasPoint(point))
                throw new IndexOutOfRangeException();

            return ref grid.Cells[point.Y * grid.Width + point.X];
        }
    }
}