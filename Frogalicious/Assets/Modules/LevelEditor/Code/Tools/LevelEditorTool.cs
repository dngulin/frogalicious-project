using Frog.LevelEditor.Data;

namespace Frog.LevelEditor.Tools
{
    internal abstract class LevelEditorTool
    {
        public abstract string Name { get; }
        public abstract void Enable(EditorLevelData level);
        public abstract void Disable();
    }
}