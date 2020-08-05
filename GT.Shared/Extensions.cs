using System;
using System.Text;

namespace GT.Shared
{
    public static class Extensions
    {
        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static string AlignString(this string value, int align)
        {
            var sb = new StringBuilder(value);
            while (sb.Length % align != 0)
            {
                sb.Append('\0');
            }
            return sb.ToString();
        }
    }
}
