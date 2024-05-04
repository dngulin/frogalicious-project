using System.Threading;
using UnityEngine;

namespace Frog.Core
{
    public class AwaitableAnimatorBehaviour : StateMachineBehaviour
    {
        private readonly AwaitableOperation _waitForStateEnter = new AwaitableOperation();
        private int _targetEnteringState;

        public Awaitable WaitForStateEnterAsync(int stateHash, CancellationToken ct)
        {
            _waitForStateEnter.TryCancel();
            _targetEnteringState = stateHash;
            return _waitForStateEnter.ExecuteAsync(ct);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_waitForStateEnter.IsRunning && stateInfo.shortNameHash == _targetEnteringState)
                _waitForStateEnter.EndAssertive();
        }
    }
}