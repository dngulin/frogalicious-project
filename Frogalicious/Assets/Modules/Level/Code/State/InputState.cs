using System;

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
}