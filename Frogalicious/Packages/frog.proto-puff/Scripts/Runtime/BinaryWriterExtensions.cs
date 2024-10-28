using System;
using System.IO;

namespace Frog.ProtoPuff
{
    public static class BinaryWriterExtensions
    {
        public static void Prepend(this BinaryWriter self, byte value)
        {
            var pos = self.BaseStream.Position - 1;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, sbyte value)
        {
            var pos = self.BaseStream.Position - 1;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, bool value)
        {
            var pos = self.BaseStream.Position - 1;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, ushort value)
        {
            var pos = self.BaseStream.Position - 2;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, short value)
        {
            var pos = self.BaseStream.Position - 2;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, uint value)
        {
            var pos = self.BaseStream.Position - 4;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, int value)
        {
            var pos = self.BaseStream.Position - 4;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, float value)
        {
            var pos = self.BaseStream.Position - 4;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, ulong value)
        {
            var pos = self.BaseStream.Position - 8;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, long value)
        {
            var pos = self.BaseStream.Position - 8;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void Prepend(this BinaryWriter self, double value)
        {
            var pos = self.BaseStream.Position - 8;
            self.Write(value);
            self.BaseStream.Position = pos;
        }

        public static void PrependLenPrefix(this BinaryWriter self, int len, out LenPrefixSize lps)
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
                self.Prepend((byte)len);
                return;
            }

            if (len <= ushort.MaxValue)
            {
                lps = LenPrefixSize._16;
                self.Prepend((ushort)len);
                return;
            }

            lps = LenPrefixSize._32;
            self.Prepend((uint)len);
        }
    }
}