using System;

namespace deleteonerror.Common.Extensions
{
  /// <summary>
  /// 
  /// </summary>
    public static class ByteArrayExtension
    {
      /// <summary>
      /// Gets the string.
      /// </summary>
      /// <param name="bytes">The bytes.</param>
      /// <returns></returns>
        public static string GetString(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}