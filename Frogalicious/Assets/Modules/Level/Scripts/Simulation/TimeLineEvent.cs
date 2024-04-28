using System.Runtime.InteropServices;
using Frog.Level.Primitives;
using UnityEngine;

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

    public static class TimeLineEventExtensions
    {
        public static ref TimeLineEvent WithEventInfo(
            this ref TimeLineEvent evt,
            TimeLineEventType type,
            ushort step,
            ushort entityId)
        {
            evt.Type = type;
            evt.Step = step;
            evt.EntityId = entityId;

            return ref evt;
        }

        public static ref TimeLineEvent WithMoveData(this ref TimeLineEvent evt, BoardPoint from, BoardPoint to)
        {
            Debug.Assert( evt.Type == TimeLineEventType.Move );
            evt.Args.AsMove = (from, to);
            return ref evt;
        }

        public static ref TimeLineEvent WithFlipFlopState(this ref TimeLineEvent evt, bool flipFlopState)
        {
            Debug.Assert( evt.Type == TimeLineEventType.FlipFlop );
            evt.Args.AsFlipFlopState = flipFlopState;
            return ref evt;
        }
    }
}