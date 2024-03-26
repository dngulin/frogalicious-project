namespace Frog.StateTracker
{
    public abstract class StateHandler<TScope> where TScope : struct
    {
        public abstract void Start(in TScope scope);
        public abstract Transition? Tick(in TScope scope);
        public abstract void Pause(in TScope scope);
        public abstract void Resume(in TScope scope);
        public abstract void Finish(in TScope scope);

        public readonly struct Transition
        {
            public readonly TransitionType Type;
            public readonly StateHandler<TScope> StateHandler;

            private Transition(TransitionType type, StateHandler<TScope> stateHandler)
            {
                Type = type;
                StateHandler = stateHandler;
            }

            public static Transition Push(StateHandler<TScope> stateHandler) => new Transition(TransitionType.Push, stateHandler);
            public static Transition Pop() => new Transition(TransitionType.Pop, null);
            public static Transition Replace(StateHandler<TScope> stateHandler) => new Transition(TransitionType.Replace, stateHandler);
        }
    }
}