namespace Frog.Core
{
    public static class NullableExtensions
    {
        public static bool TryGetValue<T>(this T? nullable, out T value) where T : struct
        {
            if (nullable.HasValue)
            {
                value = nullable.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}