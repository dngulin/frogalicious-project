using System.Runtime.InteropServices;
using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    public enum TimeLineEventType
    {
        Move,
        FlipFlop,
    }

    public struct TimeLineEvent
    {
        public TimeLineEventType Type;
        public ushort Step;
        public ushort EntityId;
        public TimeLineEventValue Value;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct TimeLineEventValue
    {
        [FieldOffset(0)] public (BoardPoint From, BoardPoint To) Move;
        [FieldOffset(0)] public bool State;
    }
}