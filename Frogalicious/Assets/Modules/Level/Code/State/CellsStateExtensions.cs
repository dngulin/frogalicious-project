using System;
using Frog.Level.Primitives;

namespace Frog.Level.State
{
    public static class CellsStateExtensions
    {
        public static bool HasPoint(this in CellsState cells, in BoardPoint point)
        {
            if (point.X < 0 || point.Y < 0)
                return false;

            if (point.X >= cells.Width || point.Y >= cells.Height)
                return false;

            return true;
        }

        public static ref readonly CellState RefReadonlyAt(this in CellsState cells, in BoardPoint point)
        {
            if (!cells.HasPoint(point))
                throw new IndexOutOfRangeException();

            return ref cells.Array.RefReadonlyAt(point.Y * cells.Width + point.X);
        }

        public static ref CellState RefAt(this ref CellsState cells, in BoardPoint point)
        {
            if (!cells.HasPoint(point))
                throw new IndexOutOfRangeException();

            return ref cells.Array.RefAt(point.Y * cells.Width + point.X);
        }
    }
}