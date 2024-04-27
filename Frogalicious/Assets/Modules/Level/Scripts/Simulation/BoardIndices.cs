using Frog.Collections;
using Frog.Level.Primitives;

namespace Frog.Level.Simulation
{
    [NoCopy]
    public struct BoardIndices
    {
        public RefList<BoardPoint> Buttons;
        public RefList<BoardPoint> Spikes;
        public RefList<BoardPoint> Springs;
    }
}