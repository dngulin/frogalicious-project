using Frog.ProtoPuff.Editor;
using UnityEditor;

namespace Frog.Core.Editor
{
    public static class SaveSystemProtoPuffGenerator
    {
        [MenuItem("Tools/Code Generation/Save System - Internal", priority = 20)]
        public static void GenerateSaveInternal()
        {
            var opts = new CodeGenOptions
            {
                Namespace = "Frog.Core.Save",
                DeserialisationApi = ApiTargets.Stream,
                SerialisationApi = ApiTargets.Stream,
            };

            CodeGenerator.Generate(
                "Assets/Modules/Core/Scripts/Save/SaveInternal.puff",
                "Assets/Modules/Core/Scripts/Save/SaveInternal.g.cs",
                opts
            );
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Code Generation/Save System - Game", priority = 21)]
        public static void GenerateSaveGame()
        {
            var opts = new CodeGenOptions
            {
                Namespace = "Frog.Core.Save",
                DeserialisationApi = ApiTargets.RefList,
                SerialisationApi = ApiTargets.RefList,
            };

            CodeGenerator.Generate(
                "Assets/Modules/Core/Scripts/Save/FrogSave.puff",
                "Assets/Modules/Core/Scripts/Save/FrogSave.g.cs",
                opts
            );
            AssetDatabase.Refresh();
        }
    }
}