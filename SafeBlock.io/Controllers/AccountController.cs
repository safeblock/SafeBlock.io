using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SafeBlock.io.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RestSharp;
using SafeBlock.io.Models.DatabaseModels;
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
        private readonly IConfiguration _configuration;
        private readonly IOptions<GlobalSettings> _globalSettings;
        private readonly IHostingEnvironment _env;
        private readonly IVaultClient _vaultClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IHostingEnvironment env, IConfiguration configuration, IOptions<GlobalSettings> globalSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _env = env;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
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
            loginSystem.Step = "register";
            
            if(ModelState.IsValid)
            {
                try
                {
                    var securityToken = SecurityUsing.CreateCryptographicallySecureGuid().ToString();
                    
                    // Chiffrement du token (avec la passphrase du site et le mot de passe utilisateur)
                    var firstCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(_globalSettings.Value.AesPassphrase, securityToken));
                    var secondCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(loginSystem.RegisterModel.Password, firstCrypt));

                    await _vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync($"safeblock/io/tokens/{SecurityUsing.Sha1(loginSystem.RegisterModel.Mail)}", new Dictionary<string, object>
                    {
                        {"token", secondCrypt},
                        {"timestamp", DateTimeOffset.Now.ToUnixTimeSeconds()}
                    });
                    
                    var newUser = new ApplicationUser()
                    {
                        UserName = loginSystem.RegisterModel.Mail.ToLower(),
                        Email = loginSystem.RegisterModel.Mail.ToLower(),
                        Token = securityToken,
                        RegisterDate = DateTime.Now,
                        HasUsingTor = SecurityUsing.IsTorVisitor(HttpContext.Connection.RemoteIpAddress.ToString()),
                        RegisterIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                        RegisterContext = JsonConvert.SerializeObject(HttpContext.Request.Headers, Formatting.Indented),
                        IsAllowed = true,
                        TwoFactorPolicy = "None"
                    };
                    
                    var creationResult = await _userManager.CreateAsync(newUser, securityToken);
                    if (creationResult.Succeeded)
                    {
                        if (!_env.IsDevelopment())
                        {
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                            var callbackUrl = Url.Page(
                                "/account/activate/",
                                pageHandler: null,
                                values: new { userId = newUser.Id, code },
                                protocol: Request.Scheme);
                            await MailUsing.SendConfirmationEmail(loginSystem.RegisterModel.Mail, callbackUrl, @"F:\SafeBlock.io\Backup\unx\SafeBlock.io\robots.txt");
                        }
                        await _signInManager.SignInAsync(newUser, loginSystem.LoginModel.KeepSession);
                        
                        return RedirectToAction("Index", "Dashboard", new {firstLogin=true});
                    }
                    foreach (var resultError in creationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, resultError.Description);
                    }
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
                var getUser = await _userManager.FindByEmailAsync(loginSystem.LoginModel.Mail);
                //ModelState.AddModelError("LoginModel.Mail", "This account does not exists.");
                
                try
                {
                    var fullyCryptedToken = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync($"safeblock/io/tokens/{SecurityUsing.Sha1(loginSystem.LoginModel.Mail)}");
                    var halfCryptedToken = Aes.Decrypt(loginSystem.LoginModel.Password, SecurityUsing.HexToBytes(fullyCryptedToken.Data.Data["token"].ToString()));
                    var token = Aes.Decrypt(_globalSettings.Value.AesPassphrase, SecurityUsing.HexToBytes(halfCryptedToken));

                    if (getUser.Token.Equals(token))
                    {
                        var loginResult = await _signInManager.PasswordSignInAsync(getUser, token, loginSystem.LoginModel.KeepSession, true);
                        if (loginResult.Succeeded)
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        if (loginResult.RequiresTwoFactor)
                        {
                            //TODO: redirect to 2FA
                        }
                        if (loginResult.IsLockedOut)
                        {
                            //TODO: redirect to lockout
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        }
                    }
                }
                catch
                {
                    ModelState.AddModelError("LoginModel.Mail", "Unable to decrypt your account.");
                }
            }
            return View("GettingStarted", loginSystem);
        }

        [Route("account/activate/{token}")]
        public async Task<IActionResult> Activate(string token)
        {
            //TODO : add expiration date
            //_context.Users.First(x => SecurityUsing.Sha512(x.Token).Equals(token)).IsMailChecked = true;
            //await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ValidateEmailAddress(string mail)
        {
            return Json(!MailUsing.IsBannedMail(mail) ? "true" : $"This email address is not allowed.");
        }
    }
}
