using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace SafeBlock.io.Services
{
    public class AuthenticationUsing
    {
        public static ClaimsPrincipal Principal(string userMail, string userRole = "User")
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, userMail));
            identity.AddClaim(new Claim(ClaimTypes.Role, userRole));
            return new ClaimsPrincipal(identity);
        }
    }
}