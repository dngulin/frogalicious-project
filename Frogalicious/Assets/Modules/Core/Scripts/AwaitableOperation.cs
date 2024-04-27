using System;
using System.Threading;
using UnityEngine;

namespace Frog.Core
{
    public class AwaitableOperation<T> : IDisposable
    {
        private readonly AwaitableCompletionSource<T> _acs = new AwaitableCompletionSource<T>();
        private CancellationTokenRegistration? _ctr;

        public Awaitable<T> ExecuteAsync(CancellationToken ct)
        {
            TryCancel();
            RegisterCancellation(ct);

            _acs.Reset();
            return _acs.Awaitable;
        }

        public bool IsRunning => _ctr != null;

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

    public class AwaitableOperation : IDisposable
    {
        private readonly AwaitableCompletionSource _acs = new AwaitableCompletionSource();
        private CancellationTokenRegistration? _ctr;

        public Awaitable ExecuteAsync(CancellationToken ct)
        {
            TryCancel();
            RegisterCancellation(ct);

            _acs.Reset();
            return _acs.Awaitable;
        }

        public bool IsRunning => _ctr != null;

        public bool TryEnd()
        {
            if (!TryUnregisterCancellation())
                return false;

            _acs.SetResult();
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
        public static void EndAssertive<T>(this AwaitableOperation<T> proc, T result)
        {
            var ended = proc.TryEnd(result);
            Debug.Assert(ended, $"Failed to end an operation with result {result}. It is not being awaited!");
        }

        public static void EndAssertive(this AwaitableOperation proc)
        {
            var ended = proc.TryEnd();
            Debug.Assert(ended, $"Failed to end an operation. It is not being awaited!");
        }
    }
}