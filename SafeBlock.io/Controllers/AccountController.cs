using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SafeBlock.io.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SafeBlock.io.Services;
using SafeBlock.io.Settings;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.SecretsEngines.PKI;

namespace SafeBlock.io.Controllers
{
    public class AccountController : Controller
    {
        private readonly SafeBlockContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IVaultClient _vaultClient;
        private readonly IUsers _users;
        private readonly IOptions<VaultSettings> _vaultSettings;

        public AccountController(IHostingEnvironment env, IOptions<VaultSettings> vaultSettings, SafeBlockContext context, IUsers users)
        {
            _env = env;
            _context = context;
            _users = users;
            _vaultSettings = vaultSettings;

            var authMethod = new TokenAuthMethodInfo(_vaultSettings.Value.Token);
            var vaultClientSettings = new VaultClientSettings(_vaultSettings.Value.ConnectionString, authMethod);
            _vaultClient = new VaultClient(vaultClientSettings); 
        }

        [Route("account/getting-started/{section?}")]
        public async Task<IActionResult> GettingStarted(string section)
        {
            // On redirige sur le tableau de bord si l'utilisateur est déja connecté
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new LoginSystem()
            {
                Step = section
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("account/getting-started/register")]
        public async Task<IActionResult> Register(LoginSystem loginSystem)
        {
            //TODO : remove user if not confirmed after 1 day
            
            loginSystem.Step = "register";

            if(ModelState.IsValid)
            {
                try
                {
                    var SecurityToken = SecurityUsing.CreateCryptographicallySecureGuid().ToString();
                    
                    var newUser = new User
                    {
                        Mail = loginSystem.RegisterModel.Mail.ToLower(),
                        Token = SecurityToken,
                        Role = "User",
                        RegisterDate = DateTime.Now,
                        HasUsingTor = SecurityUsing.IsTorVisitor(HttpContext.Connection.RemoteIpAddress.ToString()),
                        RegisterIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                        RegisterContext = JsonConvert.SerializeObject(HttpContext.Request.Headers),
                        IsAllowed = true,
                        IsMailChecked = false,
                        TwoFactorPolicy = "None",
                    };
                    _users.AddUser(newUser);
                    
                    // Chiffrement du token
                    var firstCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(_vaultSettings.Value.AESPassphrase, SecurityToken));
                    var secondCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(loginSystem.RegisterModel.Password, firstCrypt));

                    // Création du token dans le vault (doublement chiffré)
                    await _vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync($"cubbyhole/safeblock/io/{SecurityUsing.Sha1(loginSystem.RegisterModel.Mail)}", new Dictionary<string, object>
                    {
                        {"token", secondCrypt}
                    });

                    // Connexion de l'utilisateur
                    SignInUser(loginSystem.RegisterModel.Mail);

                    //TODO : changer le systeme d'envoi de mail
                    // Envoi du mail de confirmation
                    MailUsing.SendConfirmationMail(loginSystem.RegisterModel.Mail, Path.Combine(_env.ContentRootPath, "Datas", "CreateAccountMail.html"), $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/account/activate/{SecurityUsing.Sha512(SecurityToken)}", @"F:\SafeBlock.io\Backup\unx\SafeBlock.io\robots.txt");

                    return RedirectToAction("Index", "Dashboard", new {firstLogin=true});
                }
                catch
                {
                    ViewBag.CreationError = true;
                }
            }

            return View("GettingStarted", loginSystem);
        }

        // Routine de connexion
        private void SignInUser(string mail, string role = "User", bool persistent = false)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, mail));
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = persistent
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("account/getting-started/login")]
        public async Task<IActionResult> Login(LoginSystem loginSystem)
        {
            loginSystem.Step = "login";

            if (ModelState.IsValid)
            {
                var userPresence = _users.IsUserByMail(loginSystem.LoginModel.Mail);
                if (userPresence)
                {
                    var token = string.Empty;

                    try
                    {
                        var fullyCryptedToken = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync($"cubbyhole/safeblock/io/{SecurityUsing.Sha1(loginSystem.LoginModel.Mail)}");
                        var halfCryptedToken = Aes.Decrypt(loginSystem.LoginModel.Password, SecurityUsing.HexToBytes(fullyCryptedToken.Data.Data["token"].ToString()));
                        token = Aes.Decrypt(_vaultSettings.Value.AESPassphrase, SecurityUsing.HexToBytes(halfCryptedToken));
                    }
                    catch
                    {
                        ModelState.AddModelError("Mail", "Unable to decrypt your account.");
                    }

                    var user = _users.GetUserByMail(loginSystem.LoginModel.Mail);
                    if (token.ToLower().Equals(user.Token))
                    {
                        SignInUser(user.Mail, user.Role, loginSystem.LoginModel.KeepSession);

                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    ModelState.AddModelError("Mail", "This account does not exists.");
                }
            }
            return View("GettingStarted", loginSystem);
        }

        [Route("account/activate/{token}")]
        public async Task<IActionResult> Activate(string token)
        {
            //TODO : add expiration date
            _context.Users.First(x => SecurityUsing.Sha512(x.Token).Equals(token)).IsMailChecked = true;
            await _context.SaveChangesAsync();
            return View();
        }

        [Route(("account/forgot-password"))]
        public IActionResult ForgotPassword()
        {
            var loginSystem = new LoginSystem();
            loginSystem.Step = "forgot";
            return View("GettingStarted", loginSystem);
        }

        [Route("account/recover-from-certificate")]
        public IActionResult RecoverFromCertificate()
        {
            var loginSystem = new LoginSystem();
            loginSystem.Step = "recover";
            return View("GettingStarted", loginSystem);
        }

        [Route("acocunt/import-wallet")]
        public IActionResult ImportWallet()
        {
            var loginSystem = new LoginSystem();
            loginSystem.Step = "import";
            return View("GettingStarted", loginSystem);
        }

        [Route("account/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ValidateEmailAddress(string mail)
        {
            return Json(!MailUsing.IsBannedMail(mail) ? "true" : $"This email address is not allowed.");
        }
    }
}
