using System.Runtime.InteropServices;
using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    public enum TimeLineEventType
    {
        Move,
        FlipFlop,
        Disappear,
    }

    public struct TimeLineEvent
    {
        public TimeLineEventType Type;
        public ushort Step;
        public ushort EntityId;
        public TimeLineEventArgs Args;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct TimeLineEventArgs
    {
        [FieldOffset(0)] public (BoardPoint From, BoardPoint To) AsMove;
        [FieldOffset(0)] public bool AsFlipFlopState;
    }
}