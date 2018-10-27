using Microsoft.AspNetCore.Identity;
using SafeBlock.io.Models.DatabaseModels;

namespace SafeBlock.io.Extensions
{
    public class BCryptPasswordHasher : IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(SaltPassword(user, password), 10);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (BCrypt.Net.BCrypt.Verify(SaltPassword(user, providedPassword), hashedPassword))
                return PasswordVerificationResult.Success;

            return PasswordVerificationResult.Failed;
        }

        private static string SaltPassword(ApplicationUser user, string password)
        {
            return string.Concat("SafeBlock.io", password, "BBB9C97603EE06577E8E33987D7E33C4020F0779C451A27241F959559AFA3B911BBA4C22840E899BE04E7CBD5B69E51C63D524EABA3E617384EB0F74C1B9B100");
        }
    }
}