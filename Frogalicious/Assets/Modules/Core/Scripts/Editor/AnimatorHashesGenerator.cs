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
                Type = "UiAnimatorHashes",
                OutDir = "Assets/Modules/Core/Scripts/Ui",
                TestNamespace = "Frog.Core.Tests",
                TestOutDir = "Assets/Modules/Core/Scripts/Tests",
            };

            var contents = new GenContents
            {
                States = new[]
                {
                    nameof(DynUiEntityState.Appearing),
                    nameof(DynUiEntityState.Appeared),
                    nameof(DynUiEntityState.Disappeared),
                    nameof(DynUiEntityState.Disappearing),
                },
                Parameters = new[] { "IsVisible" },
                Layers = new[] { "Main" },
            };

            GenerateImpl(target, contents);
            GenerateTest(target, contents);
        }

        private static void GenerateImpl(in GenTarget target, in GenContents contents)
        {
            using var fs = File.Create($"{target.OutDir}/{target.Type}.g.cs");
            using var writer = new StreamWriter(fs);

            writer.WriteLine($"namespace {target.Namespace}");
            writer.WriteLine("{");
            {
                WriteWithIndent(writer, 4, $"public static class {target.Type}");
                WriteWithIndent(writer, 4, "{");
                {
                    WriteWithIndent(writer, 8, "public static class StateHashes");
                    WriteWithIndent(writer, 8, "{");
                    foreach (var s in contents.States)
                    {
                        WriteWithIndent(writer, 12, $"public const int {s} = {Animator.StringToHash(s)};");
                    }
                    WriteWithIndent(writer, 8, "}");

                    writer.WriteLine();

                    WriteWithIndent(writer, 8, "public static class ParamHashes");
                    WriteWithIndent(writer, 8, "{");
                    foreach (var p in contents.Parameters)
                    {
                        WriteWithIndent(writer, 12, $"public const int {p} = {Animator.StringToHash(p)};");
                    }
                    WriteWithIndent(writer, 8, "}");

                    writer.WriteLine();

                    WriteWithIndent(writer, 8, "public static class LayerIndices");
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

        private static void GenerateTest(in GenTarget target, in GenContents contents)
        {
            using var fs = File.Create($"{target.TestOutDir}/{target.Type}Tests.g.cs");
            using var writer = new StreamWriter(fs);

            writer.WriteLine("using NUnit.Framework;");
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine($"using {target.Namespace};");
            writer.WriteLine();

            writer.WriteLine($"namespace {target.TestNamespace}");
            writer.WriteLine("{");
            {
                WriteWithIndent(writer, 4, "[TestFixture]");
                WriteWithIndent(writer, 4, $"public class {target.Type}Tests");
                WriteWithIndent(writer, 4, "{");
                {
                    WriteWithIndent(writer, 8, "[Test]");
                    WriteWithIndent(writer, 8, "public void CompareHashes()");
                    WriteWithIndent(writer, 8, "{");
                    {
                        var t = target.Type;

                        foreach (var s in contents.States)
                        {
                            var line = $"Assert.That({t}.StateHashes.{s} == Animator.StringToHash(\"{s}\"));";
                            WriteWithIndent(writer, 12, line);
                        }

                        foreach (var p in contents.Parameters)
                        {
                            var line = $"Assert.That({t}.ParamHashes.{p} == Animator.StringToHash(\"{p}\"));";
                            WriteWithIndent(writer, 12, line);
                        }
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
        public string TestNamespace;
        public string TestOutDir;
    }

    internal struct GenContents
    {
        public string[] States;
        public string[] Parameters;
        public string[] Layers;
    }
}