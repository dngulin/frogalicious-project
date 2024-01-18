using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public sealed class SpikesTileView : EntityView
    {
        [SerializeField] private SpriteRenderer _renderer;

        private SpikesSprites _sprites;


        public void Init(in SpikesState state, in SpikesSprites sprites)
        {
            _sprites = sprites;
            SetActive(state.IsActive);
        }

        private void SetActive(bool isActive)
        {
            _renderer.sprite = isActive ? _sprites.Active : _sprites.Inactive;
        }

        public override void FlipFlop(bool state) => SetActive(state);
    }
}