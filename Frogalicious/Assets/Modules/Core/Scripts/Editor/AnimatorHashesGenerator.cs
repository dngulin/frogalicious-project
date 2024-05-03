using System.IO;
using Frog.Core.Ui;
using UnityEditor;
using UnityEngine;

namespace Frog.Core.Editor
{
    public static class AnimatorHashesGenerator
    {
        [MenuItem("Tools/Code Generation/Animator Hashes - AnimatedUiEntity", priority = 10)]
        public static void GenerateAnimatedUiEntityHashes()
        {
            var target = new GenTarget
            {
                Namespace = "Frog.Core.Ui",
                Type = "AnimatedUiEntityHashes",
                OutDir = "Assets/Modules/Core/Scripts/Ui",
            };

            var contents = new GenContents
            {
                States = new[]
                {
                    nameof(AnimatedUiEntityState.Appearing),
                    nameof(AnimatedUiEntityState.Appeared),
                    nameof(AnimatedUiEntityState.Disappeared),
                    nameof(AnimatedUiEntityState.Disappearing),
                },
                Parameters = new[] { "IsVisible" },
                Layers = new[] { "Main" },
            };

            Generate(target, contents);
        }

        private static void Generate(in GenTarget target, in GenContents contents)
        {
            using var fs = File.Create($"{target.OutDir}/{target.Type}.g.cs");
            using var writer = new StreamWriter(fs);

            writer.WriteLine($"namespace {target.Namespace}");
            writer.WriteLine("{");
            {
                WriteWithIndent(writer, 4, $"public static class {target.Type}");
                WriteWithIndent(writer, 4, "{");
                {
                    WriteWithIndent(writer, 8, "public static class State");
                    WriteWithIndent(writer, 8, "{");
                    foreach (var s in contents.States)
                    {
                        WriteWithIndent(writer, 12, $"public const int {s} = {Animator.StringToHash(s)};");
                    }
                    WriteWithIndent(writer, 8, "}");

                    writer.WriteLine();

                    WriteWithIndent(writer, 8, "public static class Parameter");
                    WriteWithIndent(writer, 8, "{");
                    foreach (var p in contents.Parameters)
                    {
                        WriteWithIndent(writer, 12, $"public const int {p} = {Animator.StringToHash(p)};");
                    }
                    WriteWithIndent(writer, 8, "}");

                    writer.WriteLine();

                    WriteWithIndent(writer, 8, "public static class Layer");
                    WriteWithIndent(writer, 8, "{");
                    for (var i = 0; i < contents.Layers.Length; i++)
                    {
                        WriteWithIndent(writer, 12, $"public const int {contents.Layers[i]} = {i};");
                    }
                    WriteWithIndent(writer, 8, "}");
                }
                WriteWithIndent(writer, 4, "}");
            }
            writer.WriteLine("}");
        }

        private static void WriteWithIndent(StreamWriter w, int indent, string msg)
        {
            w.Write(new string(' ', indent));
            w.WriteLine(msg);
        }
    }

    internal struct GenTarget
    {
        public string Namespace;
        public string Type;
        public string OutDir;
    }

    internal struct GenContents
    {
        public string[] States;
        public string[] Parameters;
        public string[] Layers;
    }
}