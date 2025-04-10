using System;
using Unity.Mathematics;

namespace Frog.Core
{
    public static class StringHashExtensions
    {
        public static unsafe uint XxHash32Utf16(this string self, uint seed = 0)
        {
            if (self == null)
                return 0;

            fixed (char* ptr = self)
            {
                return math.hash(ptr, self.Length * sizeof(char), seed);
            }
        }

        public static uint XxHash32Utf8(this string self, uint seed = 0)
        {
            if (self == null)
                return 0;

            var len = System.Text.Encoding.UTF8.GetByteCount(self);

            var buf = len <= 256 ? stackalloc byte[len] : new byte[len];
            System.Text.Encoding.UTF8.GetBytes(self, buf);

            return XxHash32(buf, seed);
        }

        private static unsafe uint XxHash32(ReadOnlySpan<byte> self, uint seed)
        {
            fixed (byte* ptr = self)
            {
                return math.hash(ptr, self.Length, seed);
            }
        }
    }
}