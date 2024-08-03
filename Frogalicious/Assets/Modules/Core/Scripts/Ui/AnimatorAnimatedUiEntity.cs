using System;
using System.Threading;
using UnityEngine;

using static Frog.Core.Ui.UiAnimatorHashes;

namespace Frog.Core.Ui
{
    public class AnimatorAnimatedUiEntity : AnimatedUiEntity
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        private AwaitableAnimatorBehaviour _animBehaviour;

        private void Awake()
        {
            _animator.keepAnimatorStateOnDisable = true;
            _animBehaviour = _animator.GetBehaviour<AwaitableAnimatorBehaviour>();
        }

        public override void SetVisible(bool visible)
        {
            _animBehaviour.TryCancelWaiting();
            _animator.Play(visible ? StateHashes.Appeared : StateHashes.Disappeared);
            _animator.SetBool(ParamHashes.IsVisible, visible);
        }

        public override void SetInteractable(bool interactable) => _canvasGroup.interactable = interactable;

        public override async Awaitable Show(CancellationToken ct)
        {
            if (CurrentStateHash == StateHashes.Appeared)
                return;

            _animator.SetBool(ParamHashes.IsVisible, true);
            await _animBehaviour.WaitForStateEnterAsync(StateHashes.Appeared, ct);
        }

        public override async Awaitable Hide(CancellationToken ct)
        {
            if (CurrentStateHash == StateHashes.Disappeared)
                return;

            _animator.SetBool(ParamHashes.IsVisible, false);
            await _animBehaviour.WaitForStateEnterAsync(StateHashes.Disappeared, ct);
        }

        public override AnimatedUiEntityState State
        {
            get
            {
                return CurrentStateHash switch
                {
                    StateHashes.Appearing => AnimatedUiEntityState.Appearing,
                    StateHashes.Appeared => AnimatedUiEntityState.Appeared,
                    StateHashes.Disappearing => AnimatedUiEntityState.Disappearing,
                    StateHashes.Disappeared => AnimatedUiEntityState.Disappeared,
                    _ => throw new IndexOutOfRangeException(),
                };
            }
        }

        private int CurrentStateHash
        {
            get
            {
                if (!_animator.isActiveAndEnabled)
                    return StateHashes.Disappeared;

                return _animator.GetCurrentAnimatorStateInfo(LayerIndices.Main).shortNameHash;
            }
        }
    }
}