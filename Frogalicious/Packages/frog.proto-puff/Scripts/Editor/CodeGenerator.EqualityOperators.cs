using Frog.Collections;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitEqualityOperators(in BracesScope wStruct, in PuffStruct def)
        {
            var t = def.Name;

            using (var wMethod = wStruct.Braces($"public static bool operator ==(in {t} l, in {t} r)"))
            {
                foreach (ref readonly var field in def.Fields.RefReadonlyIter())
                {
                    var f = field.Name;
                    if (!field.IsRepeated)
                    {
                        wMethod.WriteLine($"if (l.{f} != r.{f}) return false;");
                    }
                    else
                    {
                        wMethod.WriteLine($"if (l.{f}.Count() != r.{f}.Count()) return false;");

                        using var wLoop = wMethod.Braces($"for (int i = 0; i < l.{f}.Count(); i++)");
                        wLoop.WriteLine($"if (l.{f}.RefReadonlyAt(i) != r.{f}.RefReadonlyAt(i)) return false;");
                    }
                }

                wMethod.WriteLine();
                wMethod.WriteLine("return true;");
            }

            wStruct.WriteLine();
            wStruct.WriteLine($"public static bool operator !=(in {t} l, in {t} r) => !(l == r);");

            wStruct.WriteLine();
            wStruct.WriteLine("public override bool Equals(object other) => throw new NotSupportedException();");
            wStruct.WriteLine("public override int GetHashCode() => throw new NotSupportedException();");
        }
    }
}