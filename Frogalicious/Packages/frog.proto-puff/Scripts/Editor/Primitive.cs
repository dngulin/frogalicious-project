using System;

namespace Frog.ProtoPuff.Editor
{
    internal enum Primitive
    {
        I8,
        I16,
        I32,
        I64,
        U8,
        U16,
        U32,
        U64,
        F32,
        F64,
        Bool
    }

    internal static class PrimitiveExtensions
    {
        public static string CSharpName(this Primitive p)
        {
            return p switch
            {
                Primitive.I8 => "sbyte",
                Primitive.I16 => "short",
                Primitive.I32 => "int",
                Primitive.I64 => "long",
                Primitive.U8 => "byte",
                Primitive.U16 => "ushort",
                Primitive.U32 => "uint",
                Primitive.U64 => "ulong",
                Primitive.F32 => "float",
                Primitive.F64 => "double",
                Primitive.Bool => "bool",
                _ => throw new ArgumentOutOfRangeException(nameof(p), p, null),
            };
        }

        public static string SpecialisedMethodSuffix(this Primitive p)
        {
            return p switch
            {
                Primitive.I8 => "SByte",
                Primitive.I16 => "Int16",
                Primitive.I32 => "Int32",
                Primitive.I64 => "Int64",
                Primitive.U8 => "Byte",
                Primitive.U16 => "UInt16",
                Primitive.U32 => "UInt32",
                Primitive.U64 => "UInt64",
                Primitive.F32 => "Single",
                Primitive.F64 => "Double",
                Primitive.Bool => "Boolean",
                _ => throw new ArgumentOutOfRangeException(nameof(p), p, null),
            };
        }

        public static bool CanBeEnumBaseType(this Primitive p)
        {
            return p switch
            {
                Primitive.I8 => true,
                Primitive.I16 => true,
                Primitive.I32 => true,
                Primitive.I64 => true,
                Primitive.U8 => true,
                Primitive.U16 => true,
                Primitive.U32 => true,
                Primitive.U64 => true,
                _ => false,
            };
        }

        public static bool CanBeParsedFrom(this Primitive p, string value)
        {
            return p switch
            {
                Primitive.I8 => sbyte.TryParse(value, out _),
                Primitive.I16 => short.TryParse(value, out _),
                Primitive.I32 => int.TryParse(value, out _),
                Primitive.I64 => long.TryParse(value, out _),
                Primitive.U8 => byte.TryParse(value, out _),
                Primitive.U16 => ushort.TryParse(value, out _),
                Primitive.U32 => uint.TryParse(value, out _),
                Primitive.U64 => ulong.TryParse(value, out _),
                _ => false,
            };
        }

        public static string SizeOfExpr(this Primitive p)
        {
            return p switch
            {
                Primitive.I8 => "sizeof(sbyte)",
                Primitive.I16 => "sizeof(short)",
                Primitive.I32 => "sizeof(int)",
                Primitive.I64 => "sizeof(long)",
                Primitive.U8 => "sizeof(byte)",
                Primitive.U16 => "sizeof(ushort)",
                Primitive.U32 => "sizeof(uint)",
                Primitive.U64 => "sizeof(ulong)",
                Primitive.F32 => "sizeof(float)",
                Primitive.F64 => "sizeof(double)",
                Primitive.Bool => "sizeof(byte)",
                _ => throw new ArgumentOutOfRangeException(nameof(p), p, null),
            };
        }
    }
}