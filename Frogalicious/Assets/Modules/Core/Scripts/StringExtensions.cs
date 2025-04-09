using Unity.Mathematics;

namespace Frog.Core
{
    public static class StringExtensions
    {
        public static unsafe uint XxHash32(this string self, uint seed = 0)
        {
            if (self == null)
                return 0;

            fixed (char* ptr = self)
            {
                return math.hash(ptr, self.Length * sizeof(char), seed);
            }
        }
    }
}