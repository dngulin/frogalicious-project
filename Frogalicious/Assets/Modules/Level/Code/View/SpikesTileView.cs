using Frog.Level.State;
using UnityEngine;

namespace Frog.Level.View
{
    public sealed class SpikesTileView : EntityView
    {
        [SerializeField] private Animator _animator;

        private readonly int _paramIsActive = Animator.StringToHash("IsActive");

        public void Init(in SpikesState state)
        {
            SetActive(state.IsActive);
            _animator.Update(0);
        }

        public override void FlipFlop(bool state) => SetActive(state);

        private void SetActive(bool isActive)
        {
            _animator.SetBool(_paramIsActive, isActive);
        }
    }
}