using System;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitMethodSerialiseToBuf(in BracesScope wExt, in PuffStruct def)
        {
            using var wMethod = wExt.Braces($"public static void SerialiseTo(this in {def.Name} self, ref RefList<byte> buf)");
            wMethod.WriteLine("var len = self.GetSerialisedSize();");
            wMethod.WriteLine("buf.Clear();");
            wMethod.WriteLine("buf.AppendDefault(1 + ProtoPuffUtil.GetLenPrefixSize(len) + len);");
            wMethod.WriteLine("var pos = buf.Count();");
            wMethod.WriteLine();

            wMethod.WriteLine("buf.Prepend(self, ref pos);");
            wMethod.WriteLine("buf.PrependLenPrefix(len, ref pos, out var lps);");
            wMethod.WriteLine("buf.Prepend(ValueQualifier.Struct(lps).Pack(), ref pos);");
            wMethod.WriteLine();

            wMethod.WriteLine("Debug.Assert(pos == 0, $\"{pos} != 0\");");
        }

        private static void EmitMethodPrependToBuf(in BracesScope wExt, in PuffStruct def, CodeGenContext ctx)
        {
            using var wMethod = wExt.Braces($"public static void Prepend(this ref RefList<byte> buf, in {def.Name} data, ref int pos)");

            foreach (ref readonly var field in def.Fields.RefReadonlyIterReversed())
            {
                var f = field.Name;
                switch (GetValueKind(field, ctx, out var p, out var isEnum))
                {
                    case ValueKind.Primitive:
                        using (var wIf = wMethod.Braces($"if (data.{f} != default)"))
                        {
                            wIf.WriteLine(isEnum
                                ? $"buf.Prepend(({p.CSharpName()})data.{f}, ref pos);"
                                : $"buf.Prepend(data.{f}, ref pos);"
                            );
                            wIf.WriteLine($"buf.Prepend(ValueQualifier.Packed{p}, ref pos);");
                            wIf.WriteLine($"buf.Prepend((byte){field.Id}, ref pos);");
                        }
                        break;

                    case ValueKind.RepeatedPrimitive:
                        using (var wIf = wMethod.Braces($"if (data.{f}.Count() > 0)"))
                        {
                            wIf.WriteLine("var posAfterField = pos;");

                            using (var wFor = wIf.Braces($"for (int i = data.{f}.Count() - 1; i >= 0 ; i--)"))
                            {
                                wFor.WriteLine(isEnum
                                    ? $"buf.Prepend(({p.CSharpName()})data.{f}.RefReadonlyAt(i), ref pos);"
                                    : $"buf.Prepend(data.{f}.RefReadonlyAt(i), ref pos);"
                                );
                            }

                            wIf.WriteLine("var len = posAfterField - pos;");

                            wIf.WriteLine("buf.PrependLenPrefix(len, ref pos, out var lps);");
                            wIf.WriteLine($"buf.Prepend(ValueQualifier.Repeated{p}(lps).Pack(), ref pos);");
                            wIf.WriteLine($"buf.Prepend((byte){field.Id}, ref pos);");
                        }
                        break;

                    case ValueKind.Struct:
                        using (var wIf = wMethod.Braces($"if (data.{f} != default)"))
                        {
                            wIf.WriteLine("var posAfterField = pos;");
                            wIf.WriteLine($"buf.Prepend(data.{f}, ref pos);");
                            wIf.WriteLine("var len = posAfterField - pos;");

                            wIf.WriteLine("buf.PrependLenPrefix(len, ref pos, out var lps);");
                            wIf.WriteLine("buf.Prepend(ValueQualifier.Struct(lps).Pack(), ref pos);");
                            wIf.WriteLine($"buf.Prepend((byte){field.Id}, ref pos);");
                        }
                        break;

                    case ValueKind.RepeatedStruct:
                        using (var wIf = wMethod.Braces($"if (data.{f}.Count() > 0)"))
                        {
                            wIf.WriteLine("var posAfterField = pos;");
                            using (var wFor = wIf.Braces($"for (int i = data.{f}.Count() - 1; i >= 0 ; i--)"))
                            {
                                wFor.WriteLine("var posAfterItem = pos;");
                                wFor.WriteLine($"buf.Prepend(data.{f}.RefReadonlyAt(i), ref pos);");
                                wFor.WriteLine("var itemLen = posAfterItem - pos;");
                                wFor.WriteLine("buf.PrependLenPrefix(itemLen, ref pos, out var itemLps);");
                                wFor.WriteLine("buf.Prepend(ValueQualifier.Struct(itemLps).Pack(), ref pos);");
                            }
                            wIf.WriteLine("var len = posAfterField - pos;");

                            wIf.WriteLine("buf.PrependLenPrefix(len, ref pos, out var lps);");
                            wIf.WriteLine("buf.Prepend(ValueQualifier.RepeatedStruct(lps).Pack(), ref pos);");
                            wIf.WriteLine($"buf.Prepend((byte){field.Id}, ref pos);");
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                wMethod.WriteLine();
            }
        }
    }
}