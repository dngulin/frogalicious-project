using System;
using Frog.ProtoPuff.Editor.Schema;

namespace Frog.ProtoPuff.Editor
{
    public static partial class CodeGenerator
    {
        private static void EmitMethodClear(in BracesScope wExt, in StructDefinition def, CodeGenContext ctx)
        {
            using var wMethod = wExt.Braces($"public static void Clear(ref this {def.Name} self)");
            foreach (var field in def.Fields)
            {
                var f = field.Name;

                switch (GetValueKind(field, ctx, out _, out _))
                {
                    case ValueKind.Primitive:
                        wMethod.WriteLine($"self.{f} = default;");
                        break;

                    case ValueKind.RepeatedPrimitive:
                    case ValueKind.Struct:
                    case ValueKind.RepeatedStruct:
                        wMethod.WriteLine($"self.{f}.Clear();");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}