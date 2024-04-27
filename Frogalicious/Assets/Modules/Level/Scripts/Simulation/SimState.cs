using Frog.Collections;
using Frog.Level.Primitives;
using Frog.Level.State;

namespace Frog.Level.Simulation
{
    [NoCopy]
    public struct SimState
    {
        public LevelState Level;
        public BoardIndices Indices;
        public TimeLine TimeLine;
    }

    [NoCopy]
    public struct BoardIndices
    {
        public RefList<BoardPoint> Buttons;
        public RefList<BoardPoint> Spikes;
        public RefList<BoardPoint> Springs;
    }
}