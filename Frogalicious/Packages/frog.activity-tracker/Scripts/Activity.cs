using UnityEngine;

namespace Frog.ActivityTracker
{
    public abstract class Activity<TScope> where TScope : struct
    {
        public abstract void Start(in TScope scope);
        public abstract Transition? Tick(in TScope scope);
        public abstract void Pause(in TScope scope);
        public abstract void Resume(in TScope scope);
        public abstract void Finish(in TScope scope);

        public readonly struct Transition
        {
            public readonly TransitionType Type;
            public readonly Activity<TScope> Activity;

            private Transition(TransitionType type, Activity<TScope> activity)
            {
                Debug.Assert(type == TransitionType.Pop || activity != null);
                Type = type;
                Activity = activity;
            }

            public static Transition Push(Activity<TScope> activity) => new Transition(TransitionType.Push, activity);
            public static Transition Pop() => new Transition(TransitionType.Pop, null);
            public static Transition Replace(Activity<TScope> activity) => new Transition(TransitionType.Replace, activity);
        }
    }
}