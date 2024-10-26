using System;
using Frog.Collections;

namespace Frog.ProtoPuff
{
    public static class RefListReadExtensions
    {
        public static byte ReadByte(in this RefList<byte> self, ref int pos)
        {
            return self.RefReadonlyAt(pos++);
        }

        public static sbyte ReadSByte(in this RefList<byte> self, ref int pos)
        {
            return (sbyte)self.RefReadonlyAt(pos++);
        }

        public static bool ReadBoolean(in this RefList<byte> self, ref int pos)
        {
            return self.RefReadonlyAt(pos++) != 0;
        }

        public static ushort ReadUInt16(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes2
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
            };
            pos += 2;
            return bytes.Unsigned;
        }

        public static short ReadInt16(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes2
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
            };
            pos += 2;
            return bytes.Signed;
        }

        public static uint ReadUInt32(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes4
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
                _2 = self.RefReadonlyAt(pos + 2),
                _3 = self.RefReadonlyAt(pos + 3),
            };
            pos += 4;
            return bytes.Unsigned;
        }

        public static int ReadInt32(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes4
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
                _2 = self.RefReadonlyAt(pos + 2),
                _3 = self.RefReadonlyAt(pos + 3),
            };
            pos += 4;
            return bytes.Signed;
        }

        public static float ReadSingle(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes4
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
                _2 = self.RefReadonlyAt(pos + 2),
                _3 = self.RefReadonlyAt(pos + 3),
            };
            pos += 4;
            return bytes.Float;
        }

        public static ulong ReadUInt64(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes8
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
                _2 = self.RefReadonlyAt(pos + 2),
                _3 = self.RefReadonlyAt(pos + 3),
                _4 = self.RefReadonlyAt(pos + 4),
                _5 = self.RefReadonlyAt(pos + 5),
                _6 = self.RefReadonlyAt(pos + 6),
                _7 = self.RefReadonlyAt(pos + 7),
            };
            pos += 8;
            return bytes.Unsigned;
        }

        public static long ReadInt64(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes8
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
                _2 = self.RefReadonlyAt(pos + 2),
                _3 = self.RefReadonlyAt(pos + 3),
                _4 = self.RefReadonlyAt(pos + 4),
                _5 = self.RefReadonlyAt(pos + 5),
                _6 = self.RefReadonlyAt(pos + 6),
                _7 = self.RefReadonlyAt(pos + 7),
            };
            pos += 8;
            return bytes.Signed;
        }

        public static double ReadDouble(in this RefList<byte> self, ref int pos)
        {
            var bytes = new AsBytes8
            {
                _0 = self.RefReadonlyAt(pos),
                _1 = self.RefReadonlyAt(pos + 1),
                _2 = self.RefReadonlyAt(pos + 2),
                _3 = self.RefReadonlyAt(pos + 3),
                _4 = self.RefReadonlyAt(pos + 4),
                _5 = self.RefReadonlyAt(pos + 5),
                _6 = self.RefReadonlyAt(pos + 6),
                _7 = self.RefReadonlyAt(pos + 7),
            };
            pos += 8;
            return bytes.Float;
        }

        public static int ReadLenPrefix(in this RefList<byte> self, LenPrefixSize lps, ref int pos)
        {
            return lps switch
            {
                LenPrefixSize._0 => 0,
                LenPrefixSize._8 => self.ReadByte(ref pos),
                LenPrefixSize._16 => self.ReadUInt16(ref pos),
                LenPrefixSize._32 => checked((int)self.ReadUInt32(ref pos)),
                _ => throw new InvalidOperationException()
            };
        }

        public static void SkipValue(in this RefList<byte> self, ValueQualifier q, ref int pos)
        {
            var len = q.Kind switch
            {
                ValueKind.Primitive => q.PrimitiveSize.InBytes(),
                _ => self.ReadLenPrefix(q.LenPrefixSize, ref pos),
            };

            pos += len;
        }
    }
}