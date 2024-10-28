using System;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitExtMethodDeserialiseFromBuf(in BracesScope wExt, in PuffStruct def)
        {
            var t = def.Name;
            using var wMethod =
                wExt.Braces($"public static void DeserialiseFrom(this ref {t} self, in RefList<byte> buf)");

            wMethod.WriteLine("var pos = 0;");
            wMethod.WriteLine("var vq = ValueQualifier.Unpack(buf.ReadByte(ref pos));");
            wMethod.WriteLine("ProtoPuffUtil.EnsureStruct(vq);");
            wMethod.WriteLine();
            wMethod.WriteLine("self.Clear();");
            wMethod.WriteLine("var endPos = pos + buf.ReadLenPrefix(vq.LenPrefixSize, ref pos);");
            wMethod.WriteLine("self.UpdateValueFrom(buf, ref pos, endPos);");
        }

        private static void EmitExtMethodUpdateValueFromBuf(in BracesScope wExt, in PuffStruct def,
            CodeGenContext ctx)
        {
            var t = def.Name;
            var expr = $"public static void UpdateValueFrom(this ref {t} self, in RefList<byte> buf, ref int pos, int endPos)";

            using var wMethod = wExt.Braces(expr);
            using var wWhile = wMethod.Braces("while (pos < endPos)");

            wWhile.WriteLine("var fieldId = buf.ReadByte(ref pos);");

            using (var wSwitch = wWhile.Braces("switch (fieldId)"))
            {
                foreach (ref readonly var field in def.Fields.RefReadonlyIter())
                {
                    using var wCase = wSwitch.Braces($"case {field.Id}:");
                    EmitCaseParseFieldFromBuf(wCase, field, ctx);
                    wCase.WriteLine("break;");
                }

                using (var wDefault = wSwitch.Braces("default:"))
                {
                    wDefault.WriteLine("var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));");
                    wDefault.WriteLine("buf.SkipValue(fvq, ref pos);");
                    wDefault.WriteLine("break;");
                }
            }
        }

        private static void EmitCaseParseFieldFromBuf(in BracesScope wCase, in PuffField field,
            CodeGenContext ctx)
        {
            wCase.WriteLine("var fvq = ValueQualifier.Unpack(buf.ReadByte(ref pos));");

            var fn = field.Name;
            var ft = field.Type;

            switch (GetValueKind(field, ctx, out var p, out var isEnum))
            {
                case ValueKind.Primitive:
                {
                    wCase.WriteLine($"ProtoPuffUtil.Ensure{p}(fvq);");
                    wCase.WriteLine(isEnum
                        ? $"self.{fn} = ({ft})buf.Read{p.SpecialisedMethodSuffix()}(ref pos);"
                        : $"self.{fn} = buf.Read{p.SpecialisedMethodSuffix()}(ref pos);"
                    );
                    break;
                }
                case ValueKind.RepeatedPrimitive:
                {
                    wCase.WriteLine($"ProtoPuffUtil.EnsureRepeated{p}(fvq);");
                    wCase.WriteLine("var fieldEndPos = pos + buf.ReadLenPrefix(fvq.LenPrefixSize, ref pos);");
                    using var wWhile = wCase.Braces("while (pos < fieldEndPos)");
                    wWhile.WriteLine(isEnum
                        ? $"self.{fn}.RefAdd() = ({ft})buf.Read{p.SpecialisedMethodSuffix()}(ref pos);"
                        : $"self.{fn}.RefAdd() = buf.Read{p.SpecialisedMethodSuffix()}(ref pos);"
                    );
                    break;
                }
                case ValueKind.Struct:
                {
                    wCase.WriteLine("ProtoPuffUtil.EnsureStruct(fvq);");
                    wCase.WriteLine("var fieldEndPos = pos + buf.ReadLenPrefix(fvq.LenPrefixSize, ref pos);");
                    wCase.WriteLine($"self.{fn}.UpdateValueFrom(buf, ref pos, fieldEndPos);");
                    break;
                }
                case ValueKind.RepeatedStruct:
                {
                    wCase.WriteLine("ProtoPuffUtil.EnsureRepeatedStruct(fvq);");
                    wCase.WriteLine("var fieldEndPos = pos + buf.ReadLenPrefix(fvq.LenPrefixSize, ref pos);");
                    using var wWhile = wCase.Braces("while (pos < fieldEndPos)");
                    wWhile.WriteLine("var ivq = ValueQualifier.Unpack(buf.ReadByte(ref pos));");
                    wWhile.WriteLine("ProtoPuffUtil.EnsureStruct(ivq);");
                    wWhile.WriteLine("var itemEndPos = pos + buf.ReadLenPrefix(ivq.LenPrefixSize, ref pos);");
                    wWhile.WriteLine($"self.{fn}.RefAdd().UpdateValueFrom(buf, ref pos, itemEndPos);");
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