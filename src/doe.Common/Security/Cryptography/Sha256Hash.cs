using System;
using System.Security.Cryptography;
using System.Text;

namespace doe.Common.Security.Cryptography
{
    public class Sha256Hash
    {
        public static string Hash(string message, string saltValue)
        {
            return Hash(Encoding.UTF8.GetBytes(message), 
                        Encoding.UTF8.GetBytes(saltValue));
        }

        public static string Hash(byte[] plainTextBytes, byte[] saltBytes)
        {
            var hashBytes = new SHA256Managed().ComputeHash(
                Common.AddSaltToBytes(plainTextBytes,saltBytes));

            var hashWithSaltBytes = 
                Common.AddSaltToBytes(hashBytes, saltBytes);

            return Convert.ToBase64String(hashWithSaltBytes);
        }
    }
}
