using System.IO;
using System.Text;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Lexer;
using Frog.ProtoPuff.Editor.Parser;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        public static void Generate(string inputPath, string outputPath, in CodeGenOptions options)
        {
            var ctx = new CodeGenContext();

            RefList<Token> tokens;
            using (var fs = File.OpenRead(inputPath))
            {
                tokens = ProtoPuffLexer.Read(fs);
            }

            var schema = ProtoPuffParser.Parse(tokens);

            var memoryStream = new MemoryStream();
            using (var rootWriter = new StreamWriter(memoryStream, Encoding.UTF8, 2048, true))
            {
                rootWriter.WriteLine("using System;");
                rootWriter.WriteLine("using System.IO;");
                rootWriter.WriteLine("using UnityEngine;");
                rootWriter.WriteLine("using Frog.Collections;");
                rootWriter.WriteLine("using Frog.ProtoPuff;");

                rootWriter.WriteLine();

                using (var nsWriter = rootWriter.Braces($"namespace {options.Namespace}"))
                {
                    foreach (ref readonly var enumDef in schema.Enums.RefReadonlyIter())
                    {
                        EmitEnum(nsWriter, enumDef, ctx);
                        nsWriter.WriteLine();
                    }

                    foreach (ref readonly var structDef in schema.Structs.RefReadonlyIter())
                    {
                        EmitStruct(nsWriter, structDef, ctx, options);
                        nsWriter.WriteLine();
                    }
                }
            }

            using (var fs = File.Create(outputPath))
            {
                memoryStream.WriteTo(fs);
            }
        }

        private static void EmitEnum(in BracesScope wNameSpace, in PuffEnum enumDef, CodeGenContext ctx)
        {
            if (enumDef.IsFlags) wNameSpace.WriteLine("[Flags]");

            ctx.RegisterEnum(enumDef.Name, enumDef.UnderlyingType);

            using var wEnum = wNameSpace.Braces($"public enum {enumDef.Name} : {enumDef.UnderlyingType.CSharpName()}");
            foreach (ref readonly var item in enumDef.Items.RefReadonlyIter())
            {
                wEnum.WriteLine($"{item.Name} = {item.Value},");
            }
        }

        private static void EmitStruct(
            in BracesScope wNameSpace, in PuffStruct structDef, CodeGenContext ctx, in CodeGenOptions opts)
        {
            var t = structDef.Name;

            var noCopy = ctx.IsNoCopyStruct(structDef);
            if (noCopy) wNameSpace.WriteLine("[NoCopy]");

            ctx.RegisterStruct(structDef.Name, noCopy);

            using (var wStruct = wNameSpace.Braces($"public struct {t}"))
            {
                foreach (ref readonly var field in structDef.Fields.RefReadonlyIter())
                {
                    var fn = field.Name;
                    var ft = ctx.GetCSharpTypeName(field.Type);
                    wStruct.WriteLine(field.IsRepeated ? $"public RefList<{ft}> {fn};" : $"public {ft} {fn};");
                }

                wStruct.WriteLine();
                EmitEqualityOperators(wStruct, structDef);
            }

            wNameSpace.WriteLine();
            using (var wExt = wNameSpace.Braces($"public static class ProtoPuffExtensions_{t}"))
            {
                EmitMethodClear(wExt, structDef, ctx);

                if (opts.SerialisationApi != ApiTargets.None)
                {
                    wExt.WriteLine();
                    EmitMethodGetSerialisedSize(wExt, structDef, ctx);
                }

                if ((opts.SerialisationApi & ApiTargets.RefList) != ApiTargets.None)
                {
                    wExt.WriteLine();
                    EmitMethodSerialiseToBuf(wExt, structDef);
                    wExt.WriteLine();
                    EmitMethodPrependToBuf(wExt, structDef, ctx);
                }

                if ((opts.SerialisationApi & ApiTargets.Stream) != ApiTargets.None)
                {
                    wExt.WriteLine();
                    EmitMethodSerialiseToStream(wExt, structDef);
                    wExt.WriteLine();
                    EmitMethodPrependToStream(wExt, structDef, ctx);
                }

                if ((opts.DeserialisationApi & ApiTargets.RefList) != ApiTargets.None)
                {
                    wExt.WriteLine();
                    EmitExtMethodDeserialiseFromBuf(wExt, structDef);
                    wExt.WriteLine();
                    EmitExtMethodUpdateValueFromBuf(wExt, structDef, ctx);
                }

                if ((opts.DeserialisationApi & ApiTargets.Stream) != ApiTargets.None)
                {
                    wExt.WriteLine();
                    EmitExtMethodDeserialiseFromStream(wExt, structDef);
                    wExt.WriteLine();
                    EmitExtMethodUpdateValueFromStream(wExt, structDef, ctx);
                }
            }
        }

        private static ValueKind GetValueKind(in PuffField field, CodeGenContext ctx, out Primitive p, out bool isEnum)
        {
            isEnum = false;

            if (PrimitivesMap.TryGet(field.Type, out p))
            {
                return field.IsRepeated ? ValueKind.RepeatedPrimitive : ValueKind.Primitive;
            }

            if (ctx.TryGetEnumBaseType(field.Type, out p))
            {
                isEnum = true;
                return field.IsRepeated ? ValueKind.RepeatedPrimitive : ValueKind.Primitive;
            }

            return field.IsRepeated ? ValueKind.RepeatedStruct : ValueKind.Struct;
        }
    }
}