using Frog.ProtoPuff.Editor;
using UnityEditor;

namespace Frog.Core.Editor
{
    public static class SaveSystemProtoPuffGenerator
    {
        [MenuItem("Tools/Code Generation/Save System - Header", priority = 11)]
        public static void GenerateSaveInternal()
        {
            CodeGenerator.Generate(
                "Assets/Modules/Core/Scripts/Save/SaveHeader.puff",
                "Assets/Modules/Core/Scripts/Save/SaveHeader.g.cs",
                new CodeGenOptions
                {
                    Namespace = "Frog.Core.Save",
                    DeserialisationApi = ApiTargets.Stream,
                    SerialisationApi = ApiTargets.Stream,
                }
            );
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Code Generation/Save System - Data", priority = 12)]
        public static void GenerateSaveGame()
        {
            CodeGenerator.Generate(
                "Assets/Modules/Core/Scripts/Save/FrogSave.puff",
                "Assets/Modules/Core/Scripts/Save/FrogSave.g.cs",
                new CodeGenOptions
                {
                    Namespace = "Frog.Core.Save",
                    DeserialisationApi = ApiTargets.Stream,
                    SerialisationApi = ApiTargets.Stream,
                }
            );
            AssetDatabase.Refresh();
        }
    }
}