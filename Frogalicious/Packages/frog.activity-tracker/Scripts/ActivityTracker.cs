using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frog.ActivityTracker
{
    public class ActivityTracker<TScope> where TScope : struct
    {
        private readonly Stack<Activity<TScope>> _activities = new Stack<Activity<TScope>>();

        public void Start(Activity<TScope> activity, in TScope scope)
        {
            Debug.Assert(_activities.Count == 0);
            _activities.Push(activity);
            activity.Start(scope);
        }

        public bool Tick(in TScope scope)
        {
            if (_activities.Count == 0)
                return true;

            var optTransition = _activities.Peek().Tick(scope);
            if (!optTransition.TryGetValue(out var transition))
                return false;

            switch (transition.Type)
            {
                case TransitionType.Push:
                    _activities.Peek().Pause(scope);
                    StartNewActivity(transition.Activity, scope);
                    break;

                case TransitionType.Pop:
                    _activities.Pop().Finish(scope);
                    if (_activities.Count > 0)
                        _activities.Peek().Resume(scope);
                    break;

                case TransitionType.Replace:
                    _activities.Pop().Finish(scope);
                    StartNewActivity(transition.Activity, scope);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _activities.Count == 0;
        }

        private void StartNewActivity(Activity<TScope> activity, TScope scope)
        {
            Debug.Assert(activity != null, "Try to start null state handler");
            Debug.Assert(!_activities.Contains(activity), "Try to add the same state handler twice");

            _activities.Push(activity);
            activity.Start(scope);
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