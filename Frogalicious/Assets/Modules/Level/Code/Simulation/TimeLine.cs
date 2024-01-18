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
                Value = { Move = (from, to) },
            });
        }

        public readonly void AddFlipFlop(ushort entityId, bool state)
        {
            _events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.FlipFlop,
                Step = Step,
                EntityId = entityId,
                Value = { State = state },
            });
        }

        public readonly void AddDestroy(ushort entityId)
        {
            _events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.FlipFlop,
                Step = Step,
                EntityId = entityId,
                Value = default,
            });
        }
    }
}