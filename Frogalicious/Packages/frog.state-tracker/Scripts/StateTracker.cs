using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frog.StateTracker
{
    public class StateTracker<TScope> where TScope : struct
    {
        private readonly Stack<StateHandler<TScope>> _handlers = new Stack<StateHandler<TScope>>();

        public void Start(StateHandler<TScope> stateHandler, in TScope scope)
        {
            Debug.Assert(_handlers.Count == 0);
            _handlers.Push(stateHandler);
            stateHandler.Start(scope);
        }

        public bool Tick(in TScope scope)
        {
            if (_handlers.Count == 0)
                return true;

            var optTransition = _handlers.Peek().Tick(scope);
            if (!optTransition.TryGetValue(out var transition))
                return false;

            switch (transition.Type)
            {
                case TransitionType.Push:
                    _handlers.Peek().Pause(scope);
                    StartNewState(transition.StateHandler, scope);
                    break;

                case TransitionType.Pop:
                    _handlers.Pop().Finish(scope);
                    if (_handlers.Count > 0)
                        _handlers.Peek().Resume(scope);
                    break;

                case TransitionType.Replace:
                    _handlers.Pop().Finish(scope);
                    StartNewState(transition.StateHandler, scope);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _handlers.Count == 0;
        }

        private void StartNewState(StateHandler<TScope> stateHandler, TScope scope)
        {
            Debug.Assert(stateHandler != null, "Try to start null state handler");
            Debug.Assert(!_handlers.Contains(stateHandler), "Try to add the same state handler twice");

            _handlers.Push(stateHandler);
            stateHandler.Start(scope);
        }
    }

    internal static class NullableExtensions
    {
        public static bool TryGetValue<T>(this T? nullable, out T value) where T : struct
        {
            if (nullable.HasValue)
            {
                value = nullable.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}