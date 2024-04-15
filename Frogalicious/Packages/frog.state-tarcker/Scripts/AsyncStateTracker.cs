using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Frog.StateTracker
{
    public static class AsyncStateTracker
    {
        public static async Awaitable Run<TScope>(TScope scope, AsyncStateHandler<TScope> initialHandler, CancellationToken ct) where TScope : struct
        {
            var handlers = new Stack<AsyncStateHandler<TScope>>();
            handlers.Push(initialHandler);

            while (handlers.Count > 0)
            {
                ct.ThrowIfCancellationRequested();

                var transition = await handlers.Peek().Run(scope, ct);
                switch (transition.Type)
                {
                    case TransitionType.Push:
                        handlers.Push(transition.StateHandler);
                        break;

                    case TransitionType.Pop:
                        handlers.Pop();
                        break;

                    case TransitionType.Replace:
                        handlers.Pop();
                        handlers.Push(transition.StateHandler);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}