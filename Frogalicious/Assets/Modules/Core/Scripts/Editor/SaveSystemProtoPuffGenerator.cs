using Frog.ProtoPuff.Editor;
using UnityEditor;

namespace Frog.Core.Editor
{
    public static class SaveSystemProtoPuffGenerator
    {
        [MenuItem("Tools/Code Generation/Save System - Internal", priority = 20)]
        public static void GenerateSaveInternal()
        {
            CodeGenerator.Generate(
                "Assets/Modules/Core/Scripts/Save/SaveInternal.puff",
                "Assets/Modules/Core/Scripts/Save/SaveInternal.g.cs",
                "Frog.Core.Save"
            );
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Code Generation/Save System - Game", priority = 21)]
        public static void GenerateSaveGame()
        {
            CodeGenerator.Generate(
                "Assets/Modules/Core/Scripts/Save/FrogSave.puff",
                "Assets/Modules/Core/Scripts/Save/FrogSave.g.cs",
                "Frog.Core.Save"
            );
            AssetDatabase.Refresh();
        }
    }
}