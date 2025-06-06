using System;
using System.IO;

namespace Frog.ProtoPuff
{
    public static class BinaryWriterExtensions
    {
        public static void Prepend(this BinaryWriter self, byte value)
        {
            self.BaseStream.Position -= 1;
            self.Write(value);
            self.BaseStream.Position -= 1;
        }

        public static void Prepend(this BinaryWriter self, sbyte value)
        {
            self.BaseStream.Position -= 1;
            self.Write(value);
            self.BaseStream.Position -= 1;
        }

        public static void Prepend(this BinaryWriter self, bool value)
        {
            self.BaseStream.Position -= 1;
            self.Write(value);
            self.BaseStream.Position -= 1;
        }

        public static void Prepend(this BinaryWriter self, ushort value)
        {
            self.BaseStream.Position -= 2;
            self.Write(value);
            self.BaseStream.Position -= 2;
        }

        public static void Prepend(this BinaryWriter self, short value)
        {
            self.BaseStream.Position -= 2;
            self.Write(value);
            self.BaseStream.Position -= 2;
        }

        public static void Prepend(this BinaryWriter self, uint value)
        {
            self.BaseStream.Position -= 4;
            self.Write(value);
            self.BaseStream.Position -= 4;
        }

        public static void Prepend(this BinaryWriter self, int value)
        {
            self.BaseStream.Position -= 4;
            self.Write(value);
            self.BaseStream.Position -= 4;
        }

        public static void Prepend(this BinaryWriter self, float value)
        {
            self.BaseStream.Position -= 4;
            self.Write(value);
            self.BaseStream.Position -= 4;
        }

        public static void Prepend(this BinaryWriter self, ulong value)
        {
            self.BaseStream.Position -= 8;
            self.Write(value);
            self.BaseStream.Position -= 8;
        }

        public static void Prepend(this BinaryWriter self, long value)
        {
            self.BaseStream.Position -= 8;
            self.Write(value);
            self.BaseStream.Position -= 8;
        }

        public static void Prepend(this BinaryWriter self, double value)
        {
            self.BaseStream.Position -= 8;
            self.Write(value);
            self.BaseStream.Position -= 8;
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