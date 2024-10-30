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
            timeline.Events
                .RefAdd()
                .WithEventInfo(TimeLineEventType.Move, timeline.Step, entityId)
                .WithMoveData(from, to);
        }

        public static void AddFlipFlop(this ref TimeLine timeline, ushort entityId, bool state)
        {
            timeline.Events
                .RefAdd()
                .WithEventInfo(TimeLineEventType.FlipFlop, timeline.Step, entityId)
                .WithFlipFlopState(state);
        }

        public static void AddDisappear(this ref TimeLine timeline, ushort entityId)
        {
            timeline.Events.RefAdd().WithEventInfo(TimeLineEventType.Disappear, timeline.Step, entityId);
        }
    }

    public static class TimeLineCheckExtensions
    {
        public static bool IsEntityMovedThisStep(this in TimeLine timeline, ushort entityId)
        {
            foreach (ref readonly var evt in timeline.Events.RefReadonlyIterReversed())
            {
                if (evt.Step < timeline.Step)
                    break;

                if (evt.EntityId == entityId && evt.Type == TimeLineEventType.Move)
                    return true;
            }

            return false;
        }
    }
}