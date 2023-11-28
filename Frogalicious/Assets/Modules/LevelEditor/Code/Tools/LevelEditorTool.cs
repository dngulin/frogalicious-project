using Frog.Level.Data;

namespace Frog.LevelEditor.Tools
{
    internal abstract class LevelEditorTool
    {
        public abstract string Name { get; }
        public abstract void Enable(LevelData levelData);
        public abstract void Disable();
    }
}