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

            ct.Register(Cancel);

            _acs.Reset();
            _isRunning = true;

            return _acs.Awaitable;
        }

        public void End(T result)
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _acs.SetResult(result);
        }

        public void Cancel()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _acs.SetCanceled();
        }
    }
}