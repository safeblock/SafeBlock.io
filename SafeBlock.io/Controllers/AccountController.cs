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
using Microsoft.Extensions.Configuration;
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
        private readonly IOptions<GlobalSettings> _globalSettings;
        private readonly IConfiguration _configuration;

        public AccountController(IHostingEnvironment env, IConfiguration configuration, IOptions<GlobalSettings> globalSettings, SafeBlockContext context, IUsers users)
        {
            _env = env;
            _configuration = configuration;
            _context = context;
            _users = users;
            _globalSettings = globalSettings;

            var authMethod = new TokenAuthMethodInfo(configuration.GetValue<string>("VaultToken"));
            var vaultClientSettings = new VaultClientSettings(configuration.GetConnectionString("Vault"), authMethod);
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
            
            var userPresence = _users.IsUserByMail(loginSystem.RegisterModel.Mail);
            if (userPresence)
            {
                ModelState.AddModelError("RegisterModel.Mail", "This account already exists.");
            }

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
                        TwoFactorPolicy = "None"
                    };
                    _users.AddUser(newUser);
                    
                    // Chiffrement du token (avec la passphrase du site et le mot de passe utilisateur)
                    var firstCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(_globalSettings.Value.AesPassphrase, SecurityToken));
                    var secondCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(loginSystem.RegisterModel.Password, firstCrypt));

                    // Création du token dans le vault
                    await _vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync($"cubbyhole/safeblock/io/{SecurityUsing.Sha1(loginSystem.RegisterModel.Mail)}", new Dictionary<string, object>
                    {
                        {"token", secondCrypt},
                        {"timestamp", DateTimeOffset.Now.ToUnixTimeSeconds()}
                    });

                    // Connexion de l'utilisateur
                    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, AuthenticationUsing.Principal(loginSystem.RegisterModel.Mail), new AuthenticationProperties
                    {
                        IsPersistent = loginSystem.LoginModel.KeepSession
                    });

                    //TODO : changer le systeme d'envoi de mail
                    // Envoi du mail de confirmation
                    if (!_env.IsDevelopment())
                    {
                        MailUsing.SendConfirmationMail(loginSystem.RegisterModel.Mail, Path.Combine(_env.ContentRootPath, "Datas", "CreateAccountMail.html"), $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/account/activate/{SecurityUsing.Sha512(SecurityToken)}", @"F:\SafeBlock.io\Backup\unx\SafeBlock.io\robots.txt");
                    }

                    return RedirectToAction("Index", "Dashboard", new {firstLogin=true});
                }
                catch(Exception e)
                {
                    ViewBag.CreationError = true;
                    ViewBag.Exception = e.Message;
                }
            }

            return View("GettingStarted", loginSystem);
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
                        token = Aes.Decrypt(_globalSettings.Value.AesPassphrase, SecurityUsing.HexToBytes(halfCryptedToken));
                    }
                    catch
                    {
                        ModelState.AddModelError("LoginModel.Mail", "Unable to decrypt your account.");
                    }

                    var user = _users.GetUserByMail(loginSystem.LoginModel.Mail);
                    if (token.ToLower().Equals(user.Token))
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, AuthenticationUsing.Principal(user.Mail, user.Role), new AuthenticationProperties
                        {
                            IsPersistent = loginSystem.LoginModel.KeepSession
                        });

                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginModel.Mail", "This account does not exists.");
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
