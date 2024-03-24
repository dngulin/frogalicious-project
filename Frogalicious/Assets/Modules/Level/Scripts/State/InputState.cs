using System;
using Frog.Level.Primitives;

namespace Frog.Level.State
{
    [Flags]
    public enum InputState
    {
        None = 0,
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3,
    }

    public static class InputStateExtensions
    {
        public static bool HasBit(this InputState state, InputState flag) => (state & flag) != InputState.None;
    }

    public static class InputStateConvertExtensions
    {
        public static bool TryGetMoveDirection(this InputState input, out BoardDirection direction)
        {
            if (input.HasBit(InputState.Up))
            {
                direction = BoardDirection.Up;
                return true;
            }

            if (input.HasBit(InputState.Right))
            {
                direction = BoardDirection.Right;
                return true;
            }

            if (input.HasBit(InputState.Down))
            {
                direction = BoardDirection.Down;
                return true;
            }

            if (input.HasBit(InputState.Left))
            {
                direction = BoardDirection.Left;
                return true;
            }

            direction = default;
            return false;
        }
    }
}