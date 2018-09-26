using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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

        public static bool IsTorVisitor(string visitorIp)
        {
            using (var torCheck = new WebClient())
            {
                const string ip = "77.203.158.172";
                if (torCheck.DownloadString($"https://check.torproject.org/cgi-bin/TorBulkExitList.py?ip={ip}&port=").Contains(visitorIp))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
