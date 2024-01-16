using System;
using Frog.Level.Primitives;
using PlainBuffers;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

namespace Frog.Level.CodeGen
{
    public static class LevelStateGenerator
    {
        private const string InFile = "Assets/Modules/Level/Code/State/LevelState.pbs";
        private const string OutFile = "Assets/Modules/Level/Code/State/LevelState.g.cs";

        [MenuItem("Tools/Compile Level State")]
        public static void GenerateStateFile()
        {
            var namespaces = new[] { "Frog.Level.Primitives" };
            var generator = new FrogUnityCodeGenerator(namespaces);

            var structs = new[]
            {
                GetEnumInfo<BoardObjectType>(),
                GetEnumInfo<BoardTileType>(),
                ExternTypeInfo.WithoutValues(
                    nameof(BoardPoint),
                    UnsafeUtility.SizeOf<BoardPoint>(),
                    UnsafeUtility.AlignOf<BoardPoint>()
                ),
            };

            var compiler = new PlainBuffersCompiler(generator, structs);
            var (errors, warnings) = compiler.Compile(InFile, OutFile);

            foreach (var warning in warnings)
                Debug.LogWarning(warning);

            foreach (var error in errors)
                Debug.LogError(error);

            if (errors.Length == 0)
            {
                Debug.Log("LevelState compiled successfully!");
            }
        }

        private static ExternTypeInfo GetEnumInfo<T>() where T : unmanaged, Enum
        {
            var type = typeof(T);
            return ExternTypeInfo.WithEnumeratedValues(
                type.Name,
                UnsafeUtility.SizeOf<T>(),
                UnsafeUtility.AlignOf<T>(),
                Enum.GetNames(type)
            );
        }
    }
}