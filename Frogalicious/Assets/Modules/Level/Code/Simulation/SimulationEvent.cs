using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    public enum SimulationEventType
    {
        Move,
    }

    public struct SimulationEvent
    {
        public SimulationEventType Type;
        public ushort SenderId;
        public ushort Step;
        public BoardPoint Position;
        public BoardPoint EndPosition;
    }
}