using System;

namespace Frog.Level.Primitives
{
    public enum BoardDirection : byte
    {
        Up,
        Right,
        Down,
        Left,
    }

    public static class BoardDirectionExtensions
    {
        public static BoardPoint ToBoardPoint(this BoardDirection direction)
        {
            return direction switch {
                BoardDirection.Up => BoardPoint.Up,
                BoardDirection.Right => BoardPoint.Right,
                BoardDirection.Down => BoardPoint.Down,
                BoardDirection.Left => BoardPoint.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}