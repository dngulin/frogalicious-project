using Frog.Level.Data;
using Frog.Level.State;
using Frog.Level.View;
using UnityEngine;

namespace Frog.Level
{
    public partial class LevelController
    {
        public static LevelController Create(LevelData data, LevelViewConfig viewConfig, Camera camera)
        {
            var state = new LevelState(data);
            var view = new LevelView(viewConfig, data, camera);

            return new LevelController(state, view);
        }
    }
}