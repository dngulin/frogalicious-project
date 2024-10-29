using System;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitExtMethodDeserialiseFromStream(in BracesScope wExt, in PuffStruct def)
        {
            var t = def.Name;
            using var wMethod = wExt.Braces($"public static void DeserialiseFrom(this ref {t} self, BinaryReader br)");

            wMethod.WriteLine("var vq = ValueQualifier.Unpack(br.ReadByte());");
            wMethod.WriteLine("ProtoPuffUtil.EnsureStruct(vq);");
            wMethod.WriteLine();
            wMethod.WriteLine("self.Clear();");
            wMethod.WriteLine("var endPos = br.ReadLenPrefix(vq.LenPrefixSize) + br.BaseStream.Position;");
            wMethod.WriteLine("self.UpdateValueFrom(br, endPos);");
        }

        private static void EmitExtMethodUpdateValueFromStream(in BracesScope wExt, in PuffStruct def,
            CodeGenContext ctx)
        {
            var t = def.Name;
            var expr = $"public static void UpdateValueFrom(this ref {t} self, BinaryReader br, long endPos)";

            using var wMethod = wExt.Braces(expr);
            using var wWhile = wMethod.Braces("while (br.BaseStream.Position < endPos)");

            wWhile.WriteLine("var fieldId = br.ReadByte();");

            using (var wSwitch = wWhile.Braces("switch (fieldId)"))
            {
                foreach (ref readonly var field in def.Fields.RefReadonlyIter())
                {
                    using var wCase = wSwitch.Braces($"case {field.Id}:");
                    EmitCaseParseFieldFromStream(wCase, field, ctx);
                    wCase.WriteLine("break;");
                }

                using (var wDefault = wSwitch.Braces("default:"))
                {
                    wDefault.WriteLine("var fvq = ValueQualifier.Unpack(br.ReadByte());");
                    wDefault.WriteLine("br.SkipValue(fvq);");
                    wDefault.WriteLine("break;");
                }
            }
        }

        private static void EmitCaseParseFieldFromStream(in BracesScope wCase, in PuffField field,
            CodeGenContext ctx)
        {
            wCase.WriteLine("var fvq = ValueQualifier.Unpack(br.ReadByte());");

            var fn = field.Name;
            var ft = field.Type;

            switch (GetValueKind(field, ctx, out var p, out var isEnum))
            {
                case ValueKind.Primitive:
                {
                    wCase.WriteLine($"ProtoPuffUtil.Ensure{p}(fvq);");
                    wCase.WriteLine(isEnum
                        ? $"self.{fn} = ({ft})br.Read{p.SpecialisedMethodSuffix()}();"
                        : $"self.{fn} = br.Read{p.SpecialisedMethodSuffix()}();"
                    );
                    break;
                }
                case ValueKind.RepeatedPrimitive:
                {
                    wCase.WriteLine($"ProtoPuffUtil.EnsureRepeated{p}(fvq);");
                    wCase.WriteLine("var fieldEndPos = br.ReadLenPrefix(fvq.LenPrefixSize) + br.BaseStream.Position;");
                    using var wWhile = wCase.Braces("while (br.BaseStream.Position < fieldEndPos)");
                    wWhile.WriteLine(isEnum
                        ? $"self.{fn}.RefAdd() = ({ft})br.Read{p.SpecialisedMethodSuffix()}();"
                        : $"self.{fn}.RefAdd() = br.Read{p.SpecialisedMethodSuffix()}();"
                    );
                    break;
                }
                case ValueKind.Struct:
                {
                    wCase.WriteLine("ProtoPuffUtil.EnsureStruct(fvq);");
                    wCase.WriteLine("var fieldEndPos = br.ReadLenPrefix(fvq.LenPrefixSize) + br.BaseStream.Position;");
                    wCase.WriteLine($"self.{fn}.UpdateValueFrom(br, fieldEndPos);");
                    break;
                }
                case ValueKind.RepeatedStruct:
                {
                    wCase.WriteLine("ProtoPuffUtil.EnsureRepeatedStruct(fvq);");
                    wCase.WriteLine("var fieldEndPos = br.ReadLenPrefix(fvq.LenPrefixSize) + br.BaseStream.Position;");
                    using var wWhile = wCase.Braces("while (br.BaseStream.Position < fieldEndPos)");
                    wWhile.WriteLine("var ivq = ValueQualifier.Unpack(br.ReadByte());");
                    wWhile.WriteLine("ProtoPuffUtil.EnsureStruct(ivq);");
                    wWhile.WriteLine("var itemEndPos = br.ReadLenPrefix(ivq.LenPrefixSize) + br.BaseStream.Position;");
                    wWhile.WriteLine($"self.{fn}.RefAdd().UpdateValueFrom(br, itemEndPos);");
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}