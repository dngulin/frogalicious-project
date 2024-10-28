using System.Collections.Generic;


namespace Frog.ProtoPuff.Editor
{
    internal static class PrimitivesMap
    {
        private static readonly Dictionary<string, Primitive> Primitives = new()
        {
            { "i8", Primitive.I8 },
            { "i16", Primitive.I16 },
            { "i32", Primitive.I32 },
            { "i64", Primitive.I64 },
            { "u8", Primitive.U8 },
            { "u16", Primitive.U16 },
            { "u32", Primitive.U32 },
            { "u64", Primitive.U64 },
            { "f32", Primitive.F32 },
            { "f64", Primitive.F64 },
            { "bool", Primitive.Bool },
        };

        public static bool TryGet(string name, out Primitive primitive)
        {
            return Primitives.TryGetValue(name, out primitive);
        }
    }
}