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
            await handlers.PushPeek(initialHandler).Start(scope);

            while (handlers.Count > 0)
            {
                var transition = await handlers.Peek().Run(scope);
                switch (transition.Type)
                {
                    case TransitionType.Push:
                        await handlers.Peek().Pause(scope);
                        await handlers.PushPeek(transition.StateHandler).Start(scope);
                        break;

                    case TransitionType.Pop:
                        await handlers.Pop().Stop(scope);
                        if (handlers.Count > 0)
                            await handlers.Peek().Resume(scope);
                        break;

                    case TransitionType.Replace:
                        await handlers.Pop().Stop(scope);
                        await handlers.PushPeek(transition.StateHandler).Start(scope);
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