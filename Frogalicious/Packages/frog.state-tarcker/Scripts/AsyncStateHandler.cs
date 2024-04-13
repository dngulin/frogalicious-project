using System.Threading;
using UnityEngine;

namespace Frog.StateTracker
{
    public abstract class AsyncStateHandler<TScope> where TScope : struct
    {
        public abstract Awaitable<Transition> Run(TScope scope, CancellationToken ct);

        public readonly struct Transition
        {
            public readonly TransitionType Type;
            public readonly AsyncStateHandler<TScope> StateHandler;

            private Transition(TransitionType type, AsyncStateHandler<TScope> stateHandler)
            {
                Debug.Assert(type == TransitionType.Pop || stateHandler != null);
                Type = type;
                StateHandler = stateHandler;
            }

            public static Transition Push(AsyncStateHandler<TScope> stateHandler) => new Transition(TransitionType.Push, stateHandler);
            public static Transition Pop() => new Transition(TransitionType.Pop, null);
            public static Transition Replace(AsyncStateHandler<TScope> stateHandler) => new Transition(TransitionType.Replace, stateHandler);
        }
    }
}