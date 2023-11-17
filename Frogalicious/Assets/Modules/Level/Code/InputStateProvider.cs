using Frog.Level.State;
using UnityEngine.InputSystem;

namespace Frog.Level
{
    public static class InputStateProvider
    {
        public static InputState Poll()
        {
            var kbd = Keyboard.current;
            if (kbd == null)
                return InputState.None;

            var result = InputState.None;

            if (kbd.wKey.isPressed) result |= InputState.Up;
            if (kbd.dKey.isPressed) result |= InputState.Right;
            if (kbd.sKey.isPressed) result |= InputState.Down;
            if (kbd.aKey.isPressed) result |= InputState.Left;

            return result;
        }
    }
}