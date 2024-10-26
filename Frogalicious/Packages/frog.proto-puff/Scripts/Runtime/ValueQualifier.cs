namespace Frog.ProtoPuff
{
    public struct ValueQualifier
    {
        public const byte PackedI8 = (byte)PrimitiveSize._8;
        public const byte PackedI16 = (byte)PrimitiveSize._16;
        public const byte PackedI32 = (byte)PrimitiveSize._32;
        public const byte PackedI64 = (byte)PrimitiveSize._64;

        public const byte PackedU8 = (byte)PrimitiveKind.UnsignedInt << 2 | (byte)PrimitiveSize._8;
        public const byte PackedU16 = (byte)PrimitiveKind.UnsignedInt << 2 | (byte)PrimitiveSize._16;
        public const byte PackedU32 = (byte)PrimitiveKind.UnsignedInt << 2 | (byte)PrimitiveSize._32;
        public const byte PackedU64 = (byte)PrimitiveKind.UnsignedInt << 2 | (byte)PrimitiveSize._64;

        public const byte PackedF32 = (byte)PrimitiveKind.FloatPoint << 2 | (byte)PrimitiveSize._32;
        public const byte PackedF64 = (byte)PrimitiveKind.FloatPoint << 2 | (byte)PrimitiveSize._64;

        public const byte PackedBool = PackedU8;

        public ValueKind Kind;
        public LenPrefixSize LenPrefixSize;
        public PrimitiveKind PrimitiveKind;
        public PrimitiveSize PrimitiveSize;

        public ValueQualifier(ValueKind vk, LenPrefixSize lps, PrimitiveKind pk, PrimitiveSize ps)
        {
            Kind = vk;
            LenPrefixSize = lps;
            PrimitiveKind = pk;
            PrimitiveSize = ps;
        }

        public static ValueQualifier Unpack(byte packed)
        {
            ValueQualifier unpacked = default;

            const byte mask = 0b11;

            unpacked.PrimitiveSize = (PrimitiveSize)(packed & mask);

            packed = (byte)(packed >> 2);
            unpacked.PrimitiveKind = (PrimitiveKind)(packed & mask);

            packed = (byte)(packed >> 2);
            unpacked.LenPrefixSize = (LenPrefixSize)(packed & mask);

            packed = (byte)(packed >> 2);
            unpacked.Kind = (ValueKind)(packed & mask);

            return unpacked;
        }

        public static ValueQualifier RepeatedPrimitive(LenPrefixSize lps, PrimitiveKind kind, PrimitiveSize size)
        {
            return new ValueQualifier(ValueKind.RepeatedPrimitive, lps, kind, size);
        }

        public static ValueQualifier RepeatedI8(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.SignedInt, PrimitiveSize._8);
        }

        public static ValueQualifier RepeatedI16(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.SignedInt, PrimitiveSize._16);
        }

        public static ValueQualifier RepeatedI32(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.SignedInt, PrimitiveSize._32);
        }

        public static ValueQualifier RepeatedI64(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.SignedInt, PrimitiveSize._64);
        }

        public static ValueQualifier RepeatedU8(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.UnsignedInt, PrimitiveSize._8);
        }

        public static ValueQualifier RepeatedU16(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.UnsignedInt, PrimitiveSize._16);
        }

        public static ValueQualifier RepeatedU32(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.UnsignedInt, PrimitiveSize._32);
        }

        public static ValueQualifier RepeatedU64(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.UnsignedInt, PrimitiveSize._64);
        }

        public static ValueQualifier RepeatedF32(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.FloatPoint, PrimitiveSize._32);
        }

        public static ValueQualifier RepeatedF64(LenPrefixSize lps)
        {
            return RepeatedPrimitive(lps, PrimitiveKind.FloatPoint, PrimitiveSize._64);
        }

        public static ValueQualifier RepeatedBool(LenPrefixSize lps)
        {
            return RepeatedU8(lps);
        }

        public static ValueQualifier Struct(LenPrefixSize lps)
        {
            return new ValueQualifier(ValueKind.Struct, lps, default, default);
        }

        public static ValueQualifier RepeatedStruct(LenPrefixSize lps)
        {
            return new ValueQualifier(ValueKind.RepeatedStruct, lps, default, default);
        }

        public override string ToString() => $"{Kind}|{LenPrefixSize}|{PrimitiveKind}|{PrimitiveSize}";
    }

    public static class ValueQualifierExtensions
    {
        public static byte Pack(this in ValueQualifier unpacked)
        {
            var packed = (byte)unpacked.Kind;

            packed = (byte)(packed << 2);
            packed |= (byte)unpacked.LenPrefixSize;

            packed = (byte)(packed << 2);
            packed |= (byte)unpacked.PrimitiveKind;

            packed = (byte)(packed << 2);
            packed |= (byte)unpacked.PrimitiveSize;

            return packed;
        }
    }
}