using System;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitMethodSerialiseToStream(in BracesScope wExt, in PuffStruct def)
        {
            using var wMethod = wExt.Braces($"public static void SerialiseTo(this in {def.Name} self, BinaryWriter bw)");
            wMethod.WriteLine("var len = self.GetSerialisedSize();");
            wMethod.WriteLine("var prefixedLen = 1 + ProtoPuffUtil.GetLenPrefixSize(len) + len;");
            wMethod.WriteLine("bw.BaseStream.SetLength(prefixedLen);");
            wMethod.WriteLine("bw.BaseStream.Position = prefixedLen;");
            wMethod.WriteLine();

            wMethod.WriteLine("bw.Prepend(self);");
            wMethod.WriteLine("bw.PrependLenPrefix(len, out var lps);");
            wMethod.WriteLine("bw.Prepend(ValueQualifier.Struct(lps).Pack());");
            wMethod.WriteLine();

            wMethod.WriteLine("Debug.Assert(bw.BaseStream.Position == 0, $\"{bw.BaseStream.Position} != 0\");");
        }

        private static void EmitMethodPrependToStream(in BracesScope wExt, in PuffStruct def, CodeGenContext ctx)
        {
            using var wMethod = wExt.Braces($"public static void Prepend(this BinaryWriter bw, in {def.Name} data)");

            foreach (ref readonly var field in def.Fields.RefReadonlyIterReversed())
            {
                var f = field.Name;
                switch (GetValueKind(field, ctx, out var p, out var isEnum))
                {
                    case ValueKind.Primitive:
                        using (var wIf = wMethod.Braces($"if (data.{f} != default)"))
                        {
                            wIf.WriteLine(isEnum
                                ? $"bw.Prepend(({p.CSharpName()})data.{f});"
                                : $"bw.Prepend(data.{f});"
                            );
                            wIf.WriteLine($"bw.Prepend(ValueQualifier.Packed{p});");
                            wIf.WriteLine($"bw.Prepend((byte){field.Id});");
                        }
                        break;

                    case ValueKind.RepeatedPrimitive:
                        using (var wIf = wMethod.Braces($"if (data.{f}.Count() > 0)"))
                        {
                            wIf.WriteLine("var posAfterField = bw.BaseStream.Position;");

                            using (var wFor = wIf.Braces($"for (int i = data.{f}.Count() - 1; i >= 0 ; i--)"))
                            {
                                wFor.WriteLine(isEnum
                                    ? $"bw.Prepend(({p.CSharpName()})data.{f}.RefReadonlyAt(i));"
                                    : $"bw.Prepend(data.{f}.RefReadonlyAt(i));"
                                );
                            }

                            wIf.WriteLine("var len = checked((int)(posAfterField - bw.BaseStream.Position));");

                            wIf.WriteLine("bw.PrependLenPrefix(len, out var lps);");
                            wIf.WriteLine($"bw.Prepend(ValueQualifier.Repeated{p}(lps).Pack());");
                            wIf.WriteLine($"bw.Prepend((byte){field.Id});");
                        }
                        break;

                    case ValueKind.Struct:
                        using (var wIf = wMethod.Braces($"if (data.{f} != default)"))
                        {
                            wIf.WriteLine("var posAfterField = bw.BaseStream.Position;");
                            wIf.WriteLine($"bw.Prepend(data.{f});");
                            wIf.WriteLine("var len = checked((int)(posAfterField - bw.BaseStream.Position));");

                            wIf.WriteLine("bw.PrependLenPrefix(len, out var lps);");
                            wIf.WriteLine("bw.Prepend(ValueQualifier.Struct(lps).Pack());");
                            wIf.WriteLine($"bw.Prepend((byte){field.Id});");
                        }
                        break;

                    case ValueKind.RepeatedStruct:
                        using (var wIf = wMethod.Braces($"if (data.{f}.Count() > 0)"))
                        {
                            wIf.WriteLine("var posAfterField = bw.BaseStream.Position;");
                            using (var wFor = wIf.Braces($"for (int i = data.{f}.Count() - 1; i >= 0 ; i--)"))
                            {
                                wFor.WriteLine("var posAfterItem = bw.BaseStream.Position;");
                                wFor.WriteLine($"bw.Prepend(data.{f}.RefReadonlyAt(i));");
                                wFor.WriteLine("var itemLen = checked((int)(posAfterItem - bw.BaseStream.Position));");
                                wFor.WriteLine("bw.PrependLenPrefix(itemLen, out var itemLps);");
                                wFor.WriteLine("bw.Prepend(ValueQualifier.Struct(itemLps).Pack());");
                            }
                            wIf.WriteLine("var len = checked((int)(posAfterField - bw.BaseStream.Position));");

                            wIf.WriteLine("bw.PrependLenPrefix(len, out var lps);");
                            wIf.WriteLine("bw.Prepend(ValueQualifier.RepeatedStruct(lps).Pack());");
                            wIf.WriteLine($"bw.Prepend((byte){field.Id});");
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