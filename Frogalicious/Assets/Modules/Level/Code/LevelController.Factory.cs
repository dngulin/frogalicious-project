using Frog.Level.Data;
using Frog.Level.View;
using UnityEngine;

namespace Frog.Level
{
    public partial class LevelController
    {
        public static LevelController Create(LevelData data, LevelViewConfig viewConfig, Camera camera)
        {
            var view = new LevelView(viewConfig, data, camera);
            return new LevelController(view, data);
        }
    }
}