using System;
using Frog.ProtoPuff.Editor;
using Frog.ProtoPuff.Editor.Schema;
using UnityEditor;

namespace Frog.Core.Editor
{
    public static class SaveSystemProtoPuffGenerator
    {
        [MenuItem("Tools/Code Generation/Save System - Internal", priority = 20)]
        public static void GenerateSaveInternal()
        {
            var schema = new SchemaDefinition
            {
                Namespace = "Frog.Core.Save",
                Enums = Array.Empty<EnumDefinition>(),
                Structs = new []
                {
                    new StructDefinition
                    {
                        Name = "MigrationInfo",
                        Fields = new []
                        {
                            new FieldDefinition(00, "Name", "u8", true),
                            new FieldDefinition(01, "TimeStamp", "i64", false),
                        },
                    },
                    new StructDefinition
                    {
                        Name = "SaveInternal",
                        Fields = new []
                        {
                            new FieldDefinition(00, "Migrations", "MigrationInfo", true),
                            new FieldDefinition(01, "Data", "u8", true),
                        },
                    },
                },
            };

            CodeGenerator.Generate(schema, "Assets/Modules/Core/Scripts/Save/SaveInternal.g.cs");
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Code Generation/Save System - Game", priority = 21)]
        public static void GenerateSaveGame()
        {
            var schema = new SchemaDefinition
            {
                Namespace = "Frog.Core.Save",
                Enums = Array.Empty<EnumDefinition>(),
                Structs = new []
                {
                    new StructDefinition
                    {
                        Name = "FrogSave",
                        Fields = new []
                        {
                            new FieldDefinition(00, "ChapterIdx", "i32", false),
                            new FieldDefinition(01, "LevelIdx", "i32", false),
                        },
                    },
                },
            };

            CodeGenerator.Generate(schema, "Assets/Modules/Core/Scripts/Save/FrogSave.g.cs");
            AssetDatabase.Refresh();
        }
    }
}