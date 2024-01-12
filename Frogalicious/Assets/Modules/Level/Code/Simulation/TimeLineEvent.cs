using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    public enum TimeLineEventType
    {
        Move,
    }

    public struct TimeLineEvent
    {
        public TimeLineEventType Type;
        public ushort Step;
        public BoardPoint Position;
        public BoardPoint EndPosition;
    }
}