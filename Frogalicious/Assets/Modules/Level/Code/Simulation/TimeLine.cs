using System.Collections.Generic;
using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    public struct TimeLine
    {
        private readonly List<TimeLineEvent> _events;
        public ushort Step;

        public readonly bool IsEmpty => _events.Count == 0;

        public TimeLine(List<TimeLineEvent> events)
        {
            _events = events;
            Step = 0;
        }

        public readonly void AddMove(ushort entityId, BoardPoint from, BoardPoint to)
        {
            _events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.Move,
                Step = Step,
                EntityId = entityId,
                Args = { AsMove = (from, to) },
            });
        }

        public readonly void AddFlipFlop(ushort entityId, bool state)
        {
            _events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.FlipFlop,
                Step = Step,
                EntityId = entityId,
                Args = { AsFlipFlopState = state },
            });
        }

        public readonly void AddDisappear(ushort entityId)
        {
            _events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.Disappear,
                Step = Step,
                EntityId = entityId,
                Args = default,
            });
        }

        public readonly bool IsEntityMovedThisStep(ushort entityId)
        {
            for (var i = _events.Count - 1; i >= 0; i--)
            {
                var evt = _events[i];
                if (evt.Step < Step)
                    break;

                if (evt.EntityId == entityId && evt.Type == TimeLineEventType.Move)
                    return true;
            }

            return false;
        }
    }
}