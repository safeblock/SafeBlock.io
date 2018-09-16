using System;

namespace SafeBlock.io.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public DateTime? RegisterDate { get; set; }
        public bool? HasUsingTor { get; set; }
        public string RegisterIp { get; set; }
        public string RegisterContext { get; set; }
        public bool? IsAllowed { get; set; }
        public bool? IsMailChecked { get; set; }
        public string TwoFactorPolicy { get; set; }
    }
}
