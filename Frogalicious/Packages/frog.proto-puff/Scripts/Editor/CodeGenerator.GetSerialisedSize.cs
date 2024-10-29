using System;
using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitMethodGetSerialisedSize(in BracesScope wExt, in PuffStruct def, CodeGenContext ctx)
        {
            using var wMethod = wExt.Braces($"public static int GetSerialisedSize(this in {def.Name} self)");
            wMethod.WriteLine("var result = 0;");
            wMethod.WriteLine();

            foreach (ref readonly var field in def.Fields.RefReadonlyIter())
            {
                var f = field.Name;

                switch (GetValueKind(field, ctx, out var p, out _))
                {
                    case ValueKind.Primitive:
                        using (var wIf = wMethod.Braces($"if (self.{f} != default)"))
                        {
                            wIf.WriteLine($"result += 2 + {p.SizeOfExpr()};");
                        }
                        break;

                    case ValueKind.RepeatedPrimitive:
                        using (var wIf = wMethod.Braces($"if (self.{f}.Count() > 0)"))
                        {
                            wIf.WriteLine($"var len = {p.SizeOfExpr()} * self.{f}.Count();");
                            wIf.WriteLine("result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;");
                        }
                        break;

                    case ValueKind.Struct:
                        using (var wIf = wMethod.Braces($"if (self.{f} != default)"))
                        {
                            wIf.WriteLine($"var len = self.{f}.GetSerialisedSize();");
                            wIf.WriteLine("result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;");
                        }
                        break;

                    case ValueKind.RepeatedStruct:
                        using (var wIf = wMethod.Braces($"if (self.{f}.Count() > 0)"))
                        {
                            wIf.WriteLine("var len = 0;");

                            using (var wFor = wIf.Braces($"for (int i = 0; i < self.{f}.Count(); i++)"))
                            {
                                wFor.WriteLine($"var itemLen = self.{f}.RefReadonlyAt(i).GetSerialisedSize();");
                                wFor.WriteLine("len += 1 + ProtoPuffUtil.GetLenPrefixSize(itemLen) + itemLen;");
                            }

                            wIf.WriteLine("result += 2 + ProtoPuffUtil.GetLenPrefixSize(len) + len;");
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                wMethod.WriteLine();
            }

            wMethod.WriteLine("return result;");
        }
    }
}