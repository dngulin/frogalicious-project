using System.Runtime.CompilerServices;
using UnityEngine;

namespace Frog.ProtoPuff
{
    public static class ProtoPuffUtil
    {
        public static int GetLenPrefixSize(int len)
        {
            Debug.Assert(len >= 0);

            return len switch
            {
                0 => 0,
                <= byte.MaxValue => 1,
                <= ushort.MaxValue => 2,
                _ => 4
            };
        }

        public static void EnsureStruct(ValueQualifier vq)
        {
            if (vq.Kind != ValueKind.Struct)
                throw new ProtoPuffException($"Struct value is expected, got {vq}");
        }

        public static void EnsureRepeatedStruct(ValueQualifier vq)
        {
            if (vq.Kind != ValueKind.RepeatedStruct)
                throw new ProtoPuffException($"Repeated struct is expected, got {vq}");
        }

        public static void EnsureI8(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_8(vq);
        }

        public static void EnsureRepeatedI8(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_8(vq);
        }

        public static void EnsureI16(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_16(vq);
        }

        public static void EnsureRepeatedI16(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_16(vq);
        }

        public static void EnsureI32(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_32(vq);
        }

        public static void EnsureRepeatedI32(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_32(vq);
        }

        public static void EnsureI64(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_64(vq);
        }

        public static void EnsureRepeatedI64(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureSignedInt(vq);
            EnsureSize_64(vq);
        }

        public static void EnsureU8(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_8(vq);
        }

        public static void EnsureRepeatedU8(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_8(vq);
        }

        public static void EnsureU16(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_16(vq);
        }

        public static void EnsureRepeatedU16(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_16(vq);
        }

        public static void EnsureU32(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_32(vq);
        }

        public static void EnsureRepeatedU32(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_32(vq);
        }

        public static void EnsureU64(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_64(vq);
        }

        public static void EnsureRepeatedU64(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureUnsignedInt(vq);
            EnsureSize_64(vq);
        }

        public static void EnsureF32(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureFloatPoint(vq);
            EnsureSize_32(vq);
        }

        public static void EnsureRepeatedF32(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureFloatPoint(vq);
            EnsureSize_32(vq);
        }

        public static void EnsureF64(ValueQualifier vq)
        {
            EnsurePrimitive(vq);
            EnsureFloatPoint(vq);
            EnsureSize_64(vq);
        }

        public static void EnsureRepeatedF64(ValueQualifier vq)
        {
            EnsureRepeatedPrimitive(vq);
            EnsureFloatPoint(vq);
            EnsureSize_64(vq);
        }

        public static void EnsureBool(ValueQualifier vq) => EnsureU8(vq);

        public static void EnsureRepeatedBool(ValueQualifier vq) => EnsureRepeatedU8(vq);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsurePrimitive(ValueQualifier vq)
        {
            if (vq.Kind != ValueKind.Primitive)
                throw new ProtoPuffException($"Primitive is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureRepeatedPrimitive(ValueQualifier vq)
        {
            if (vq.Kind != ValueKind.RepeatedPrimitive)
                throw new ProtoPuffException($"Repeated primitive is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureSignedInt(ValueQualifier vq)
        {
            if (vq.PrimitiveKind != PrimitiveKind.SignedInt)
                throw new ProtoPuffException($"Signed integer is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureUnsignedInt(ValueQualifier vq)
        {
            if (vq.PrimitiveKind != PrimitiveKind.UnsignedInt)
                throw new ProtoPuffException($"Unsigned integer is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureFloatPoint(ValueQualifier vq)
        {
            if (vq.PrimitiveKind != PrimitiveKind.FloatPoint)
                throw new ProtoPuffException($"Float point number is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureSize_8(ValueQualifier vq)
        {
            if (vq.PrimitiveSize != PrimitiveSize._8)
                throw new ProtoPuffException($"8-bit value is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureSize_16(ValueQualifier vq)
        {
            if (vq.PrimitiveSize != PrimitiveSize._16)
                throw new ProtoPuffException($"16-bit value is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureSize_32(ValueQualifier vq)
        {
            if (vq.PrimitiveSize != PrimitiveSize._32)
                throw new ProtoPuffException($"32-bit value is expected, got {vq}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureSize_64(ValueQualifier vq)
        {
            if (vq.PrimitiveSize != PrimitiveSize._64)
                throw new ProtoPuffException($"64-bit value is expected, got {vq}");
        }
    }
}