using Frog.Level.Data;
using Frog.Level.Simulation;
using Frog.Level.View;

namespace Frog.Level
{
    public partial class LevelController
    {
        public static LevelController Create(LevelData data, LevelViewConfig viewConfig)
        {
            var simulation = new LevelSimulation(data);
            var view = new LevelView(viewConfig, data);

            var state = simulation.CreateInitialState();

            return new LevelController(state, simulation, view);
        }
    }
}