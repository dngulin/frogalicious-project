using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frog.StateTracker
{
    public class AsyncStateTracker<TScope> where TScope : struct
    {
        private readonly Stack<AsyncStateHandler<TScope>> _handlers = new Stack<AsyncStateHandler<TScope>>();

        public async Awaitable Run(TScope scope, AsyncStateHandler<TScope> initialHandler)
        {
            await _handlers.PushPeek(initialHandler).Start(scope);

            while (_handlers.Count > 0)
            {
                var transition = await _handlers.Peek().Run(scope);
                switch (transition.Type)
                {
                    case TransitionType.Push:
                        await _handlers.Peek().Pause(scope);
                        await _handlers.PushPeek(transition.StateHandler).Start(scope);
                        break;

                    case TransitionType.Pop:
                        await _handlers.Pop().Stop(scope);
                        if (_handlers.Count > 0)
                            await _handlers.Peek().Resume(scope);
                        break;

                    case TransitionType.Replace:
                        await _handlers.Pop().Stop(scope);
                        await _handlers.PushPeek(transition.StateHandler).Start(scope);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    internal static class StackExtensions
    {
        public static T PushPeek<T>(this Stack<T> stack, T item)
        {
            stack.Push(item);
            return item;
        }
    }
}