namespace SafeBlock.io.Models
{
    public class LoginSystem
    {
        public HandleRegisterModel RegisterModel { get; set; }
        public HandleLoginModel LoginModel { get; set; }
        public string Step { get; set; } = "login";
    }
}