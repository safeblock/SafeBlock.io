using System;
using Microsoft.AspNetCore.Identity;

namespace SafeBlock.io.Models.DatabaseModels
{
    public class ApplicationUser : IdentityUser
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool? HasUsingTor { get; set; }
        public string RegisterIp { get; set; }
        public string RegisterContext { get; set; }
        public string TwoFactorPolicy { get; set; }
        public bool? IsAllowed { get; set; }
    }
}