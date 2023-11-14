using System;
using Frog.Level.Primitives;
using Frog.Level.State;

namespace Frog.Level.Simulation
{
    public enum MoveDirection
    {
        Up,
        Right,
        Down,
        Left,
    }

    public static class MoveDirectionExtensions
    {
        public static BoardPoint ToBoardPoint(this MoveDirection direction)
        {
            return direction switch {
                MoveDirection.Up => BoardPoint.Up,
                MoveDirection.Right => BoardPoint.Right,
                MoveDirection.Down => BoardPoint.Down,
                MoveDirection.Left => BoardPoint.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }

    public static class InputStateConvertExtensions
    {
        public static bool TryGetMoveDirection(this InputState input, out MoveDirection direction)
        {
            if (input.HasBit(InputState.Up))
            {
                direction = MoveDirection.Up;
                return true;
            }

            if (input.HasBit(InputState.Right))
            {
                direction = MoveDirection.Right;
                return true;
            }

            if (input.HasBit(InputState.Down))
            {
                direction = MoveDirection.Down;
                return true;
            }

            if (input.HasBit(InputState.Left))
            {
                direction = MoveDirection.Left;
                return true;
            }

            direction = default;
            return false;
        }
    }
}