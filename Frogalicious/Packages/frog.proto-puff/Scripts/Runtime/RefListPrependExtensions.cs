using System;
using Frog.Collections;

namespace Frog.ProtoPuff
{
    public static class RefListPrependExtensions
    {
        public static void Prepend(ref this RefList<byte> self, byte value, ref int pos)
        {
            self.RefAt(--pos) = value;
        }

        public static void Prepend(ref this RefList<byte> self, sbyte value, ref int pos)
        {
            self.RefAt(--pos) = (byte)value;
        }

        public static void Prepend(ref this RefList<byte> self, bool value, ref int pos)
        {
            self.RefAt(--pos) = value ? (byte)1 : (byte)0;
        }

        public static void Prepend(ref this RefList<byte> self, ushort value, ref int pos)
        {
            var bytes = new AsBytes2 { Unsigned = value };
            self.RefAt(pos - 1) = bytes._1;
            self.RefAt(pos - 2) = bytes._0;
            pos -= 2;
        }

        public static void Prepend(ref this RefList<byte> self, short value, ref int pos)
        {
            var bytes = new AsBytes2 { Signed = value };
            self.RefAt(pos - 1) = bytes._1;
            self.RefAt(pos - 2) = bytes._0;
            pos -= 2;
        }

        public static void Prepend(ref this RefList<byte> self, uint value, ref int pos)
        {
            var bytes = new AsBytes4 { Unsigned = value };
            self.RefAt(pos - 1) = bytes._3;
            self.RefAt(pos - 2) = bytes._2;
            self.RefAt(pos - 3) = bytes._1;
            self.RefAt(pos - 4) = bytes._0;
            pos -= 4;
        }

        public static void Prepend(ref this RefList<byte> self, int value, ref int pos)
        {
            var bytes = new AsBytes4 { Signed = value };
            self.RefAt(pos - 1) = bytes._3;
            self.RefAt(pos - 2) = bytes._2;
            self.RefAt(pos - 3) = bytes._1;
            self.RefAt(pos - 4) = bytes._0;
            pos -= 4;
        }

        public static void Prepend(ref this RefList<byte> self, float value, ref int pos)
        {
            var bytes = new AsBytes4 { Float = value };
            self.RefAt(pos - 1) = bytes._3;
            self.RefAt(pos - 2) = bytes._2;
            self.RefAt(pos - 3) = bytes._1;
            self.RefAt(pos - 4) = bytes._0;
            pos -= 4;
        }

        public static void Prepend(ref this RefList<byte> self, ulong value, ref int pos)
        {
            var bytes = new AsBytes8 { Unsigned = value };
            self.RefAt(pos - 1) = bytes._7;
            self.RefAt(pos - 2) = bytes._6;
            self.RefAt(pos - 3) = bytes._5;
            self.RefAt(pos - 4) = bytes._4;
            self.RefAt(pos - 5) = bytes._3;
            self.RefAt(pos - 6) = bytes._2;
            self.RefAt(pos - 7) = bytes._1;
            self.RefAt(pos - 8) = bytes._0;
            pos -= 8;
        }

        public static void Prepend(ref this RefList<byte> self, long value, ref int pos)
        {
            var bytes = new AsBytes8 { Signed = value };
            self.RefAt(pos - 1) = bytes._7;
            self.RefAt(pos - 2) = bytes._6;
            self.RefAt(pos - 3) = bytes._5;
            self.RefAt(pos - 4) = bytes._4;
            self.RefAt(pos - 5) = bytes._3;
            self.RefAt(pos - 6) = bytes._2;
            self.RefAt(pos - 7) = bytes._1;
            self.RefAt(pos - 8) = bytes._0;
            pos -= 8;
        }

        public static void Prepend(ref this RefList<byte> self, double value, ref int pos)
        {
            var bytes = new AsBytes8 { Float = value };
            self.RefAt(pos - 1) = bytes._7;
            self.RefAt(pos - 2) = bytes._6;
            self.RefAt(pos - 3) = bytes._5;
            self.RefAt(pos - 4) = bytes._4;
            self.RefAt(pos - 5) = bytes._3;
            self.RefAt(pos - 6) = bytes._2;
            self.RefAt(pos - 7) = bytes._1;
            self.RefAt(pos - 8) = bytes._0;
            pos -= 8;
        }

        public static void PrependLenPrefix(ref this RefList<byte> self, int len, ref int pos, out LenPrefixSize lps)
        {
            if (len < 0)
                throw new InvalidOperationException("Try to prepend negative len prefix");

            if (len == 0)
            {
                lps = LenPrefixSize._0;
                return;
            }

            if (len <= byte.MaxValue)
            {
                lps = LenPrefixSize._8;
                self.Prepend((byte)len, ref pos);
                return;
            }

            if (len <= ushort.MaxValue)
            {
                lps = LenPrefixSize._16;
                self.Prepend((ushort)len, ref pos);
                return;
            }

            lps = LenPrefixSize._32;
            self.Prepend((uint)len, ref pos);
        }
    }
}