using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frog.StateTracker
{
    public class AsyncStateTracker<TScope> where TScope : struct
    {
        private readonly Stack<AsyncStateHandler<TScope>> _handlers = new Stack<AsyncStateHandler<TScope>>();

        public async Awaitable Run(TScope scope, AsyncStateHandler<TScope> initialState)
        {
            _handlers.Push(initialState);

            while (_handlers.Count > 0)
            {
                var transition = await _handlers.Peek().Run(scope);
                switch (transition.Type)
                {
                    case TransitionType.Push:
                        _handlers.Push(transition.StateHandler);
                        break;

                    case TransitionType.Pop:
                        _handlers.Pop();
                        break;

                    case TransitionType.Replace:
                        _handlers.Pop();
                        _handlers.Push(transition.StateHandler);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}