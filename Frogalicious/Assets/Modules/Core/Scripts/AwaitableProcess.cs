using System.Threading;
using UnityEngine;

namespace Frog.Core
{
    public class AwaitableProcess<T>
    {
        private readonly AwaitableCompletionSource<T> _acs = new AwaitableCompletionSource<T>();
        private bool _isRunning;

        public Awaitable<T> Begin(CancellationToken ct)
        {
            if (_isRunning)
            {
                _acs.SetCanceled();
            }

            ct.Register(() => TryCancel());

            _acs.Reset();
            _isRunning = true;

            return _acs.Awaitable;
        }

        public bool TryEnd(T result)
        {
            if (!_isRunning)
                return false;

            _isRunning = false;
            _acs.SetResult(result);
            return true;
        }

        public bool TryCancel()
        {
            if (!_isRunning)
                return false;

            _isRunning = false;
            _acs.SetCanceled();
            return true;
        }
    }
}