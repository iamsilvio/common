using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace doe.Common.Security.Cryptography
{
    /// <summary>
    /// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and
    /// decrypt data. As long as it is initialized with the same constructor
    /// parameters, the class will use the same key. Before performing encryption,
    /// the class can prepend random bytes to plain text and generate different
    /// encrypted values from the same plain text, encryption key, initialization
    /// vector, and other parameters. This class is thread-safe.
    /// </summary>
    public class RijndaelEnhanced
    {
        private readonly ICryptoTransform _encryptor;
        private readonly ICryptoTransform _decryptor;

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with the key derived from the explicitly specified
        /// parameters.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="minSaltLen">
        /// Min size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is less than 4, the default min value will be used (currently 4
        /// bytes).
        /// </param>
        /// <param name="maxSaltLen">
        /// Max size (in bytes) of randomly generated salt which will be added at
        /// the beginning of plain text before encryption is performed. When this
        /// value is negative or greater than 255, the default max value will be
        /// used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        /// than the specified min value (which can be adjusted to default value),
        /// salt will not be used and plain text value will be encrypted as is.
        /// In this case, salt will not be processed during decryption either.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to hash password. More iterations are
        /// considered more secure but may take longer.
        /// </param>
        public RijndaelEnhanced(string passPhrase,
                                       string initVector,
                                       int minSaltLen = 8,
                                       int maxSaltLen = 255,
                                       int keySize = 256,
                                       int passwordIterations = 1)
            : this(passPhrase, 
                   initVector, 
                   Common.GenerateSalt(
                        Common.GenerateRandomNumber(minSaltLen,maxSaltLen)), 
                   keySize, 
                   passwordIterations)
        {}


        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with the key derived from the explicitly specified
        /// parameters.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used for password hashing during key generation. This is
        /// not the same as the salt we will use during encryption. This parameter
        /// can be any string.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to hash password. More iterations are
        /// considered more secure but may take longer.
        /// </param>
        public RijndaelEnhanced(string passPhrase, 
                                string initVector,
                                string saltValue,
                                int keySize = 256,
                                int passwordIterations = 1)
            : this(passPhrase, initVector, Encoding.UTF8.GetBytes(saltValue), 
                   keySize, passwordIterations)
        {
        }

        /// <summary>
        /// Use this constructor if you are planning to perform encryption/
        /// decryption with the key derived from the explicitly specified
        /// parameters.
        /// </summary>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived.
        /// The derived password will be used to generate the encryption key
        /// Passphrase can be any string. In this example we assume that the
        /// passphrase is an ASCII string. Passphrase value must be kept in
        /// secret.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long. IV value does not have to be kept
        /// in secret.
        /// </param>
        /// <param name="keySize">
        /// Size of symmetric key (in bits): 128, 192, or 256.
        /// </param>
        /// <param name="saltValueBytes">
        /// Salt value used for password hashing during key generation. This is
        /// not the same as the salt we will use during encryption. This parameter
        /// can be any string.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to hash password. More iterations are
        /// considered more secure but may take longer.
        /// </param>
        public RijndaelEnhanced(string passPhrase, 
                                string initVector,
                                byte[] saltValueBytes,
                                int keySize = 256, 
                                int passwordIterations = 1)
        {
       
            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            var password = new Rfc2898DeriveBytes(passPhrase, 
                                                  saltValueBytes, 
                                                  passwordIterations);

            // Convert key to a byte array adjusting the size from bits to bytes.
            var keyBytes = password.GetBytes(keySize/8);

            var symmetricKey = new RijndaelManaged
            {
                Mode = initVectorBytes.Length == 0
                    ? CipherMode.ECB
                    : CipherMode.CBC
            };

            // If we do not have initialization vector, we cannot use the CBC mode.
            // The only alternative is the ECB mode (which is not as good).

            // Create encryptor and decryptor, which we will use for cryptographic
            // operations.
            _encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            _decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        }

        #region Encryption routines

        /// <summary>
        /// Encrypts a string value generating a base64-encoded string.
        /// </summary>
        /// <param name="plainText">
        /// Plain text string to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a base64-encoded string.
        /// </returns>
        public string Encrypt(string plainText)
        {
            return Encrypt(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Encrypts a byte array generating a base64-encoded string.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Plain text bytes to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a base64-encoded string.
        /// </returns>
        public string Encrypt(byte[] plainTextBytes)
        {
            return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
        }

        /// <summary>
        /// Encrypts a string value generating a byte array of cipher text.
        /// </summary>
        /// <param name="plainText">
        /// Plain text string to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a byte array.
        /// </returns>
        public byte[] EncryptToBytes(string plainText)
        {
            return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
        }

        /// <summary>
        /// Encrypts a byte array generating a byte array of cipher text.
        /// </summary>
        /// <param name="plainTextBytes">
        /// Plain text bytes to be encrypted.
        /// </param>
        /// <returns>
        /// Cipher text formatted as a byte array.
        /// </returns>
        public byte[] EncryptToBytes(byte[] plainTextBytes)
        {
            var saltLength = Common.GenerateRandomNumber(8, 16);

            var plainTextBytesWithSalt = 
                Common.AddSaltToBytes(plainTextBytes, saltLength);
            
            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                lock (this)
                {
                    using (var cryptoStream =
                        new CryptoStream(memoryStream,
                            _encryptor,
                            CryptoStreamMode.Write))
                    {
                        cryptoStream.Write( plainTextBytesWithSalt,
                                            0,
                                            plainTextBytesWithSalt.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memoryStream.ToArray();
                    }
                }
            }
            return cipherTextBytes;
        }

        #endregion

        #region Decryption routines

        /// <summary>
        /// Decrypts a base64-encoded cipher text value generating a string result.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-encoded cipher text string to be decrypted.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        public string Decrypt(string cipherText)
        {
            return Decrypt(Convert.FromBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts a byte array containing cipher text value and generates a
        /// string result.
        /// </summary>
        /// <param name="cipherTextBytes">
        /// Byte array containing encrypted data.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        public string Decrypt(byte[] cipherTextBytes)
        {
            return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
        }

        /// <summary>
        /// Decrypts a base64-encoded cipher text value and generates a byte array
        /// of plain text data.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-encoded cipher text string to be decrypted.
        /// </param>
        /// <returns>
        /// Byte array containing decrypted value.
        /// </returns>
        public byte[] DecryptToBytes(string cipherText)
        {
            return DecryptToBytes(Convert.FromBase64String(cipherText));
        }

        /// <summary>
        /// Decrypts a base64-encoded cipher text value and generates a byte array
        /// of plain text data.
        /// </summary>
        /// <param name="cipherTextBytes">
        /// Byte array containing encrypted data.
        /// </param>
        /// <returns>
        /// Byte array containing decrypted value.
        /// </returns>
        public byte[] DecryptToBytes(byte[] cipherTextBytes)
        {
            int decryptedByteCount;
            var saltLen = 0;
            var decryptedBytes = new byte[cipherTextBytes.Length];

            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
                lock (this)
                {
                    using (var cryptoStream =
                        new CryptoStream(memoryStream,
                            _decryptor,
                            CryptoStreamMode.Read))
                    {
                        decryptedByteCount = 
                            cryptoStream.Read(decryptedBytes,
                                              0,
                                              decryptedBytes.Length);
                    }
                }
            }

            // If we are using salt, get its length from the first 4 bytes of 
            // plain text data.
            //if (_maxSaltLen > 0 && _maxSaltLen >= _minSaltLen)
            //{
                saltLen = (decryptedBytes[0] & 0x03) |
                          (decryptedBytes[1] & 0x0c) |
                          (decryptedBytes[2] & 0x30) |
                          (decryptedBytes[3] & 0xc0);
            //}

            // Allocate the byte array to hold the original plain text (without salt).
            var plainTextBytes = new byte[decryptedByteCount - saltLen];

            // Copy original plain text discarding the salt value if needed.
            Array.Copy(decryptedBytes, saltLen, plainTextBytes,
                0, decryptedByteCount - saltLen);

            // Return original plain text value.
            return plainTextBytes;
        }

        #endregion

    }
}
