using System.Threading;
using UnityEngine;

namespace Frog.ActivityTracker
{
    public abstract class AsyncActivity<TScope> where TScope : struct
    {
        public abstract Awaitable<Transition> ExecuteAsync(TScope scope, CancellationToken ct);

        public abstract void Tick(in TScope scope, float dt);

        public abstract void Dispose(in TScope scope);

        public readonly struct Transition
        {
            public readonly TransitionType Type;
            public readonly AsyncActivity<TScope> Activity;

            private Transition(TransitionType type, AsyncActivity<TScope> activity)
            {
                Debug.Assert(type == TransitionType.Pop || activity != null);
                Type = type;
                Activity = activity;
            }

            public static Transition Push(AsyncActivity<TScope> activity) => new Transition(TransitionType.Push, activity);
            public static Transition Pop() => new Transition(TransitionType.Pop, null);
            public static Transition Replace(AsyncActivity<TScope> activity) => new Transition(TransitionType.Replace, activity);
        }
    }
}