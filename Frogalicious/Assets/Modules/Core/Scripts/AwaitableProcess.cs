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
            RegisterCancellation(ct);

            _acs.Reset();
            return _acs.Awaitable;
        }

        public bool TryEnd(T result)
        {
            if (!TryUnregisterCancellation())
                return false;

            _acs.SetResult(result);
            return true;
        }

        public bool TryCancel()
        {
            if (!TryUnregisterCancellation())
                return false;

            _acs.SetCanceled();
            return true;
        }

        public void Dispose() => TryCancel();

        private void RegisterCancellation(CancellationToken ct)
        {
            Debug.Assert(_ctr == null, "CancellationTokenRegistration will be overwritten!");
            _ctr = ct.Register(() => TryCancel());
        }

        private bool TryUnregisterCancellation()
        {
            if (_ctr == null)
                return false;

            _ctr.Value.Dispose();
            _ctr = null;
            return true;
        }
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