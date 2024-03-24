using System;
using Frog.Level.Simulation;
using UnityEngine;

namespace Frog.Level.View
{
    public readonly struct TimelineJob
    {
        private const float StepDuration = 0.5f;
        private const float HalfStepDuration = StepDuration / 2;

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

            var midTime = _startTime + HalfStepDuration;
            var endTime = _startTime + StepDuration;

            switch (_jobType)
            {
                case TimeLineEventType.Move:
                    var factor = Mathf.Clamp01((currTime - _startTime) / StepDuration);
                    var args = _jobArgs.AsMove;
                    _target.transform.position = Vector2.Lerp(args.From.ToVector2(), args.To.ToVector2(), factor);
                    break;

                case TimeLineEventType.FlipFlop:
                    if (prevTime <= midTime && currTime > midTime)
                    {
                        _target.FlipFlop(_jobArgs.AsFlipFlopState);
                    }
                    break;

                case TimeLineEventType.Disappear:
                    if (prevTime <= midTime && currTime > midTime)
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