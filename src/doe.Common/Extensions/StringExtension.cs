using System;

namespace doe.Common.Extensions
{
  /// <summary>
  /// 
  /// </summary>
    public static class StringExtension
    {
      /// <summary>
      /// Gets the bytes.
      /// </summary>
      /// <param name="str">The string.</param>
      /// <returns></returns>
        public static byte[] GetBytes(this String str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}