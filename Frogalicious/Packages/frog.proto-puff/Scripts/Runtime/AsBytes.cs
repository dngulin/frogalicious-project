using System.Runtime.InteropServices;

namespace Frog.ProtoPuff
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct AsBytes2
    {
        [FieldOffset(0)] public ushort Unsigned;
        [FieldOffset(0)] public short Signed;

        [FieldOffset(0)] public byte _0;
        [FieldOffset(1)] public byte _1;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct AsBytes4
    {
        [FieldOffset(0)] public uint Unsigned;
        [FieldOffset(0)] public int Signed;
        [FieldOffset(0)] public float Float;

        [FieldOffset(0)] public byte _0;
        [FieldOffset(1)] public byte _1;
        [FieldOffset(2)] public byte _2;
        [FieldOffset(3)] public byte _3;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct AsBytes8
    {
        [FieldOffset(0)] public ulong Unsigned;
        [FieldOffset(0)] public long Signed;
        [FieldOffset(0)] public double Float;

        [FieldOffset(0)] public byte _0;
        [FieldOffset(1)] public byte _1;
        [FieldOffset(2)] public byte _2;
        [FieldOffset(3)] public byte _3;
        [FieldOffset(4)] public byte _4;
        [FieldOffset(5)] public byte _5;
        [FieldOffset(6)] public byte _6;
        [FieldOffset(7)] public byte _7;
    }
}