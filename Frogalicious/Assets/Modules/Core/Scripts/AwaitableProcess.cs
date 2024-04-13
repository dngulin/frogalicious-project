using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core
{
    public class AwaitableProcess<T> : IDisposable
    {
        private readonly AwaitableCompletionSource<T> _acs = new AwaitableCompletionSource<T>();
        private CancellationTokenRegistration? _ctr;

        public Awaitable<T> Begin(CancellationToken ct)
        {
            TryCancel();

            _acs.Reset();

            Debug.Assert(_ctr == null);
            _ctr = ct.Register(() => TryCancel());

            return _acs.Awaitable;
        }

        public bool TryEnd(T result)
        {
            if (_ctr == null)
                return false;

            _ctr.Value.Dispose();
            _ctr = null;

            _acs.SetResult(result);
            return true;
        }

        public bool TryCancel()
        {
            if (_ctr == null)
                return false;

            _ctr.Value.Dispose();
            _ctr = null;

            _acs.SetCanceled();
            return true;
        }

        public void Dispose() => TryCancel();
    }

    public static class AwaitableProcessExtensions
    {
        public static void EndWithAssert<T>(this AwaitableProcess<T> proc, T result)
        {
            var ended = proc.TryEnd(result);
            Debug.Assert(ended, $"Failed to end the process with result {result}. Process is not being awaited!");
        }
    }
}