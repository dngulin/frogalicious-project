using Frog.Collections;
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
}