using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Frog.StateTracker
{
    public class AsyncStateTracker<TScope> where TScope : struct
    {
        private readonly Stack<AsyncStateHandler<TScope>> _handlers = new Stack<AsyncStateHandler<TScope>>();

        public async Awaitable Run(TScope scope, AsyncStateHandler<TScope> initialHandler, CancellationToken ct)
        {
            Debug.Assert(_handlers.Count == 0);
            _handlers.Push(initialHandler);

            while (_handlers.Count > 0)
            {
                ct.ThrowIfCancellationRequested();

                var transition = await _handlers.Peek().Run(scope, ct);
                switch (transition.Type)
                {
                    case TransitionType.Push:
                        _handlers.Push(transition.StateHandler);
                        break;

                    case TransitionType.Pop:
                        _handlers.Pop().Dispose(scope);
                        break;

                    case TransitionType.Replace:
                        _handlers.Pop().Dispose(scope);
                        _handlers.Push(transition.StateHandler);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Dispose(TScope scope)
        {
            while (_handlers.Count > 0)
            {
                _handlers.Pop().Dispose(scope);
            }
        }
    }
}