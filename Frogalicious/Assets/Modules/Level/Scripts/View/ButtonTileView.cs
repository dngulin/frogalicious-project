using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public sealed class ButtonTileView : EntityView
    {
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField]private Sprite _pressed;
        [SerializeField] private Sprite _normal;

        public ButtonTileView Initialized(in ButtonState state)
        {
            SetPressed(state.IsPressed);
            return this;
        }

        public override void FlipFlop(bool state) => SetPressed(state);

        private void SetPressed(bool isPressed)
        {
            _renderer.sprite = isPressed ? _pressed : _normal;
        }
    }
}