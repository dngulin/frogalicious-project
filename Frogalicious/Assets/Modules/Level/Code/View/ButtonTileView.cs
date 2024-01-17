using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public sealed class ButtonTileView : EntityView
    {
        [SerializeField] private SpriteRenderer _renderer;

        private ButtonSprites _sprites;

        public void Init(in ButtonState state, in ButtonSprites sprites)
        {
            _sprites = sprites;
            SetPressed(state.IsPressed);
        }

        public void SetPressed(bool isPressed)
        {
            _renderer.sprite = isPressed ? _sprites.Pressed : _sprites.Normal;
        }
    }
}