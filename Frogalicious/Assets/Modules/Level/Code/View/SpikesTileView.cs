using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public sealed class SpikesTileView : EntityView
    {
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] private Sprite _active;
        [SerializeField] private Sprite _inactive;

        public void Init(in SpikesState state) => SetActive(state.IsActive);
        public override void FlipFlop(bool state) => SetActive(state);

        private void SetActive(bool isActive)
        {
            _renderer.sprite = isActive ? _active : _inactive;
        }
    }
}