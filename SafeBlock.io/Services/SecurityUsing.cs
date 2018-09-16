using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace SafeBlock.io.Services
{
    public class SecurityUsing
    {
        public static string HashBCrypt(string password)
        {
            var saltedPassword = Sha1(string.Concat(Sha1(password), password, Sha1(password)));
            return Encoding.UTF8.GetString(Aes.Encrypt("pute", BCrypt.Net.BCrypt.HashPassword(saltedPassword)));
        }

        public static bool VerifyBCrypt(string password, string plainPassword)
        {
            var saltedPassword = Sha1(string.Concat(Sha1(plainPassword), plainPassword, Sha1(plainPassword)));
            var UnAES = Aes.Decrypt("pute", Encoding.UTF8.GetBytes(password));
            return BCrypt.Net.BCrypt.Verify(saltedPassword, UnAES);
        }
        
        public static Guid CreateCryptographicallySecureGuid() 
        {
            using (var provider = new RNGCryptoServiceProvider()) 
            {
                var bytes = new byte[16];
                provider.GetBytes(bytes);

                return new Guid(bytes);
            }
        }

        public static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }
        
        public static byte[] HexToBytes(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        
        public static string Sha1(string value)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(value));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
        
        public static string Sha512(string value)
        {
            var hash = (new SHA512Managed()).ComputeHash(Encoding.UTF8.GetBytes(value));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }
    }

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