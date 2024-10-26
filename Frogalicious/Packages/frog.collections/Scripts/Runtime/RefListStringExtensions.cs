using System.Text;

namespace Frog.Collections
{
    public static class RefListStringExtensions
    {
        public static void SetFromUtf8String(this ref RefList<byte> list, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                list.Clear();
                return;
            }

            var len = Encoding.UTF8.GetByteCount(value);
            list.SetSize(len);

            list.ItemCount = Encoding.UTF8.GetBytes(value, 0, value.Length, list.ItemArray, 0);
        }

        public static void SetFromAsciiString(this ref RefList<byte> list, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                list.Clear();
                return;
            }

            list.SetSize(value.Length);
            list.ItemCount = Encoding.ASCII.GetBytes(value, 0, value.Length, list.ItemArray, 0);
        }

        public static string ToStringUtf8(this in RefList<byte> list)
        {
            return list.ItemCount == 0 ? "" : Encoding.UTF8.GetString(list.ItemArray, 0, list.ItemCount);
        }

        public static string ToStringAscii(this in RefList<byte> list)
        {
            return list.ItemCount == 0 ? "" : Encoding.ASCII.GetString(list.ItemArray, 0, list.ItemCount);
        }
    }
}