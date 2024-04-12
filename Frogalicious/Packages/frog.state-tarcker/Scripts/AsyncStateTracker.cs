using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frog.StateTracker
{
    public static class AsyncStateTracker<TScope> where TScope : struct
    {
        public static async Awaitable Run(TScope scope, AsyncStateHandler<TScope> initialHandler)
        {
            var handlers = new Stack<AsyncStateHandler<TScope>>();
            handlers.Push(initialHandler);

            while (handlers.Count > 0)
            {
                var transition = await handlers.Peek().Run(scope);
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