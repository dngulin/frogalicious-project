using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Frog.ActivityTracker
{
    public class AsyncActivityTracker<TScope>
    {
        private readonly Stack<AsyncActivity<TScope>> _activities = new Stack<AsyncActivity<TScope>>();

        public async Awaitable ExecuteAsync(TScope scope, AsyncActivity<TScope> initialActivity, CancellationToken ct)
        {
            Debug.Assert(_activities.Count == 0);
            _activities.Push(initialActivity);

            while (_activities.Count > 0)
            {
                ct.ThrowIfCancellationRequested();

                var transition = await _activities.Peek().ExecuteAsync(scope, ct);
                switch (transition.Type)
                {
                    case TransitionType.Push:
                        _activities.Push(transition.Activity);
                        break;

                    case TransitionType.Pop:
                        _activities.Pop().Dispose(scope);
                        break;

                    case TransitionType.Replace:
                        _activities.Pop().Dispose(scope);
                        _activities.Push(transition.Activity);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Tick(in TScope scope, float dt)
        {
            if (_activities.Count > 0)
            {
                _activities.Peek().Tick(scope, dt);
            }
        }

        public void Dispose(in TScope scope)
        {
            while (_activities.Count > 0)
            {
                _activities.Pop().Dispose(scope);
            }
        }
    }
}