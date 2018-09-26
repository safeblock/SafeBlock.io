using System.IO;
using System.Security.Claims;
using ConsulSharp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SafeBlock.io.Services
{
    public static class AuthenticationUsing
    {
        public static IConfigurationRoot Configuration { get; set; }
        
        public static ClaimsPrincipal Principal(string userMail, string userRole = "User")
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, userMail));
            identity.AddClaim(new Claim(ClaimTypes.Role, userRole));
            return new ClaimsPrincipal(identity);
        }
        
        #region Secured Session Backend
        public static void SetSecuredString(this ISession session, string key, string value)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            session.SetString(key, SecurityUsing.BytesToHex(Aes.Encrypt(Configuration.GetSection("Global").GetValue<string>("AesPassphrase"), value)));
        }

        public static string GetSecuredString(this ISession session, string key)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            return Aes.Decrypt(Configuration.GetSection("Global").GetValue<string>("AesPassphrase"),
                SecurityUsing.HexToBytes(session.GetString(key)));
        }
        #endregion
    }
}