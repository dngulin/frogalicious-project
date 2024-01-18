using System;
using Frog.Level.Simulation;
using UnityEngine;

namespace Frog.Level.View
{
    public readonly struct TimelineJob
    {
        private const float StepDuration = 0.5f;

        private readonly float _startTime;
        private readonly TimeLineEventType _jobType;
        private readonly TimeLineEventArgs _jobArgs;
        private readonly EntityView _target;

        public TimelineJob(TimeLineEvent evt, EntityView target)
        {
            _startTime = evt.Step * StepDuration;
            _jobType = evt.Type;
            _jobArgs = evt.Args;
            _target = target;
        }

        public bool Update(float prevTime, float dt)
        {
            var currTime = prevTime + dt;
            if (currTime < _startTime)
                return false;

            var endTime = _startTime + StepDuration;

            switch (_jobType)
            {
                case TimeLineEventType.Move:
                    var factor = Mathf.Clamp01((currTime - _startTime) / StepDuration);
                    var args = _jobArgs.AsMove;
                    _target.transform.position = Vector2.Lerp(args.From.ToVector2(), args.To.ToVector2(), factor);
                    break;

                case TimeLineEventType.FlipFlop:
                    if (prevTime <= _startTime && currTime > _startTime)
                    {
                        _target.FlipFlop(_jobArgs.AsFlipFlopState);
                    }
                    break;

                case TimeLineEventType.Disappear:
                    if (prevTime <= _startTime && currTime > _startTime)
                    {
                        _target.Disappear();
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return currTime >= endTime;
        }
    }
}