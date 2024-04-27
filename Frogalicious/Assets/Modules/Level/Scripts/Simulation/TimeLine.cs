using Frog.Collections;
using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    [NoCopy]
    public struct TimeLine
    {
        public RefList<TimeLineEvent> Events;
        public ushort Step;

        public TimeLine(int capacity)
        {
            Events = RefList.WithCapacity<TimeLineEvent>(capacity);
            Step = 0;
        }
    }

    public static class TimeLineExtensions
    {
        public static void Reset(this ref TimeLine timeline)
        {
            timeline.Events.Clear();
            timeline.Step = 0;
        }
    }

    public static class TimeLineEventsExtensions
    {
        public static void AddMove(this ref TimeLine timeline, ushort entityId, BoardPoint from, BoardPoint to)
        {
            timeline.Events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.Move,
                Step = timeline.Step,
                EntityId = entityId,
                Args = { AsMove = (from, to) },
            });
        }

        public static void AddFlipFlop(this ref TimeLine timeline, ushort entityId, bool state)
        {
            timeline.Events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.FlipFlop,
                Step = timeline.Step,
                EntityId = entityId,
                Args = { AsFlipFlopState = state },
            });
        }

        public static void AddDisappear(this ref TimeLine timeline, ushort entityId)
        {
            timeline.Events.Add(new TimeLineEvent
            {
                Type = TimeLineEventType.Disappear,
                Step = timeline.Step,
                EntityId = entityId,
                Args = default,
            });
        }
    }

    public static class TimeLineCheckExtensions
    {
        public static bool IsEntityMovedThisStep(this in TimeLine timeline, ushort entityId)
        {
            for (var i = timeline.Events.Count() - 1; i >= 0; i--)
            {
                ref readonly var evt = ref timeline.Events.RefReadonlyAt(i);
                if (evt.Step < timeline.Step)
                    break;

                if (evt.EntityId == entityId && evt.Type == TimeLineEventType.Move)
                    return true;
            }

            return false;
        }
    }
}