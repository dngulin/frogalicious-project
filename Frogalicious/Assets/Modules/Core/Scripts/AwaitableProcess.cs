using UnityEngine;

namespace Frog.Core
{
    public class AwaitableProcess<T>
    {
        private readonly AwaitableCompletionSource<T> _acs = new AwaitableCompletionSource<T>();
        private bool _isRunning;

        public Awaitable<T> Begin()
        {
            if (_isRunning)
            {
                _acs.SetCanceled();
            }

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
    }
}