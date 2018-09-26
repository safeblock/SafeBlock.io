using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SafeBlock.io.Services
{
    public static class Aes
    {
        public const int KEY_SIZE = 16;

        public static byte[] Encrypt (string password, string input)
        {
            var sha256CryptoServiceProvider = new SHA256CryptoServiceProvider();
            var hash = sha256CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
            var key = new byte[KEY_SIZE];
            var iv = new byte[KEY_SIZE];

            Buffer.BlockCopy(hash, 0, key, 0, KEY_SIZE);
            Buffer.BlockCopy(hash, KEY_SIZE, iv, 0, KEY_SIZE);

            using (var cipher = new AesCryptoServiceProvider().CreateEncryptor(key, iv))
            using (var output = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(output, cipher, CryptoStreamMode.Write))
                {
                    var inputBytes = Encoding.UTF8.GetBytes(input);
                    cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                }
                return output.ToArray();
            }
        }

        public static string Decrypt(string password, byte[] encryptedBytes)
        {
            var sha256CryptoServiceProvider = new SHA256CryptoServiceProvider();
            var hash = sha256CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(password));
            var key = new byte[KEY_SIZE];
            var iv = new byte[KEY_SIZE];

            Buffer.BlockCopy(hash, 0, key, 0, KEY_SIZE);
            Buffer.BlockCopy(hash, KEY_SIZE, iv, 0, KEY_SIZE);

            using (var cipher = new AesCryptoServiceProvider().CreateDecryptor(key, iv))
            using (var source = new MemoryStream(encryptedBytes))
            using (var output = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(source, cipher, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(output);
                }
                return Encoding.UTF8.GetString(output.ToArray());
            }
        }
    }
}
