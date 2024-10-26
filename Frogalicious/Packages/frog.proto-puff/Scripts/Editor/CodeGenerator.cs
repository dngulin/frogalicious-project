using System.IO;
using System.Text;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        public static void Generate(in SchemaDefinition schema, string outputPath)
        {
            var ctx = new CodeGenContext();
            var memoryStream = new MemoryStream();

            using (var rootWriter = new StreamWriter(memoryStream, Encoding.UTF8, 2048, true))
            {
                rootWriter.WriteLine("using System;");
                rootWriter.WriteLine("using System.IO;");
                rootWriter.WriteLine("using UnityEngine;");
                rootWriter.WriteLine("using Frog.Collections;");
                rootWriter.WriteLine("using Frog.ProtoPuff;");

                rootWriter.WriteLine();

                using (var nsWriter = rootWriter.Braces($"namespace {schema.Namespace}"))
                {
                    foreach (var enumDef in schema.Enums)
                    {
                        EmitEnum(nsWriter, enumDef, ctx);
                        nsWriter.WriteLine();
                    }

                    foreach (var structDef in schema.Structs)
                    {
                        EmitStruct(nsWriter, structDef, ctx);
                        nsWriter.WriteLine();
                    }
                }
            }

            using (var fs = File.Create(outputPath))
            {
                memoryStream.WriteTo(fs);
            }
        }

        private static void EmitEnum(in BracesScope wNameSpace, in EnumDefinition enumDef, CodeGenContext ctx)
        {
            if (enumDef.Flags) wNameSpace.WriteLine("[Flags]");

            var enumType = ValidationUtil.GetEnumUnderlyingType(enumDef.UnderlyingType);

            ctx.ValidateNewTypeName(enumDef.Name);
            ctx.RegisterEnum(enumDef.Name, enumType);

            using var wEnum = wNameSpace.Braces($"public enum {enumDef.Name} : {enumType.CSharpName()}");
            foreach (var item in enumDef.Items)
            {
                ValidationUtil.ValidateEnumItemName(item.Name, enumDef);
                ValidationUtil.ValidateEnumItemValue(item, enumDef, enumType);
                wEnum.WriteLine($"{item.Name} = {item.Value},");
            }
        }

        private static void EmitStruct(in BracesScope wNameSpace, in StructDefinition structDef, CodeGenContext ctx)
        {
            var t = structDef.Name;

            var noCopy = ctx.IsNoCopyStruct(structDef);
            if (noCopy) wNameSpace.WriteLine("[NoCopy]");

            ctx.ValidateNewTypeName(structDef.Name);
            ctx.RegisterStruct(structDef.Name, noCopy);

            using (var wStruct = wNameSpace.Braces($"public struct {t}"))
            {
                foreach (var field in structDef.Fields)
                {
                    ValidationUtil.ValidateFieldName(field.Name, structDef);
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

                wExt.WriteLine();
                EmitMethodGetSerialisedSize(wExt, structDef, ctx);

                wExt.WriteLine();
                EmitMethodSerialiseToBuf(wExt, structDef);
                wExt.WriteLine();
                EmitMethodPrependToBuf(wExt, structDef, ctx);

                wExt.WriteLine();
                EmitMethodSerialiseToStream(wExt, structDef);
                wExt.WriteLine();
                EmitMethodPrependToStream(wExt, structDef, ctx);

                wExt.WriteLine();
                EmitExtMethodDeserialiseFromBuf(wExt, structDef);
                wExt.WriteLine();
                EmitExtMethodUpdateValueFromBuf(wExt, structDef, ctx);

                wExt.WriteLine();
                EmitExtMethodDeserialiseFromStream(wExt, structDef);
                wExt.WriteLine();
                EmitExtMethodUpdateValueFromStream(wExt, structDef, ctx);
            }
        }

        private static ValueKind GetValueKind(in FieldDefinition field, CodeGenContext ctx, out Primitive p, out bool isEnum)
        {
            isEnum = false;

            if (ValidationUtil.TryGetPrimitiveType(field.Type, out p))
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