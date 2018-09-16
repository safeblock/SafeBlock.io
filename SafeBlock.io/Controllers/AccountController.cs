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
using VaultSharp.Backends.Authentication.Models.Token;

namespace SafeBlock.io.Controllers
{
    public class AccountController : Controller
    {
        private SafeBlockContext _context;
        private IHostingEnvironment _env;
        private IVaultClient _vaultClient;
        
        private readonly IUsers _users;

        public AccountController(IHostingEnvironment env, IOptions<VaultSettings> _vaultSettings, SafeBlockContext context, IUsers users)
        {
            _env = env;
            _context = context;

            _vaultClient = VaultClientFactory.CreateVaultClient(new Uri(_vaultSettings.Value.ConnectionString), new TokenAuthenticationInfo(_vaultSettings.Value.Token));

            _users = users;
        }

        [Route("account/getting-started/{section?}")]
        public IActionResult GettingStarted(string section)
        {
            // On redirige sur le tableau de bord si l'utilisateur est déja connecté
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Section = section;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("account/getting-started/register")]
        public async Task<IActionResult> Register(HandleLoginModel handleLoginModel)
        {
            //TODO : remove user if not confirmed after 1 day

            ViewBag.Section = "register";
            ModelState.Remove("KeepSession");

            if(ModelState.IsValid)
            {
                using (var sb = new SafeBlockContext())
                {
                    var SecurityToken = SecurityUsing.CreateCryptographicallySecureGuid().ToString();

                    try
                    {
                        var newUser = new User
                        {
                            Mail = handleLoginModel.Mail.ToLower(),
                            Token = SecurityToken,    //SecurityUsing.HashBCrypt(handleLoginModel.Password),
                            Role = "User",
                            RegisterDate = DateTime.Now,
                            HasUsingTor = true,    //TODO : detect tor ip
                            RegisterIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                            RegisterContext = JsonConvert.SerializeObject(HttpContext.Request.Headers),
                            IsAllowed = true,
                            IsMailChecked = false,
                            TwoFactorPolicy = "None",
                        };
                        _users.AddUser(newUser);
                        
                        //TODO : store in secret
                        // Chiffrement du token
                        var firstCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(
                            "8a174f91ebc1713f62108712267eca28dcf8bcc12d155c9dd79cd30661a7a1d665350330c074cd9cd9c702ba7e750192188aca5fefbb942e822862da9c4c7dba",
                            SecurityToken));
                        var secondCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(
                            handleLoginModel.Password,
                            firstCrypt));

                        // Création du token dans le vault (doublement chiffré)
                        await _vaultClient.WriteSecretAsync($"cubbyhole/safeblock/io/{SecurityUsing.Sha1(handleLoginModel.Mail)}", new Dictionary<string, object>
                        {
                            {"token", secondCrypt}
                        });

                        // Connexion de l'utilisateur
                        SignInUser(handleLoginModel.Mail);

                        // Envoi du mail de confirmation
                        MailUsing.SendConfirmationMail(handleLoginModel.Mail, Path.Combine(_env.ContentRootPath, "Datas", "CreateAccountMail.html"), $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/account/activate/{SecurityUsing.Sha512(SecurityToken)}", @"F:\SafeBlock.io\Backup\unx\SafeBlock.io\robots.txt");

                        return RedirectToAction("ChooseWallet", "Dashboard");
                    }
                    catch// (DbUpdateException e)
                    {
                        ModelState.AddModelError("Mail", "This mail address is already used.");
                    }
                }
            }

            return View("GettingStarted", handleLoginModel);
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
        public async Task<IActionResult> Login(HandleLoginModel handleLoginModel)
        {
            ViewBag.Section = "login";
            ModelState.Remove("VerifyPassword");

            if (ModelState.IsValid)
            {
                var userPresence = _users.IsUserByMail(handleLoginModel.Mail);
                if (userPresence)
                {
                    var token = string.Empty;

                    try
                    {
                        var fullyCryptedToken = await _vaultClient.ReadSecretAsync($"cubbyhole/safeblock/io/{SecurityUsing.Sha1(handleLoginModel.Mail)}");
                        var halfCryptedToken = Aes.Decrypt(handleLoginModel.Password, SecurityUsing.HexToBytes(fullyCryptedToken.Data["token"].ToString()));
                        token = Aes.Decrypt("8a174f91ebc1713f62108712267eca28dcf8bcc12d155c9dd79cd30661a7a1d665350330c074cd9cd9c702ba7e750192188aca5fefbb942e822862da9c4c7dba", SecurityUsing.HexToBytes(halfCryptedToken));
                    }
                    catch
                    {
                        ModelState.AddModelError("Mail", "Unable to decrypt your account.");
                    }

                    var user = _users.GetUserByMail(handleLoginModel.Mail);
                    if (token.ToLower().Equals(user.Token))
                    {
                        SignInUser(user.Mail, user.Role, handleLoginModel.KeepSession);

                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    ModelState.AddModelError("Mail", "This account does not exists.");
                }
            }
            return View("GettingStarted", handleLoginModel);
        }

        [Route("account/activate/{token}")]
        public async Task<IActionResult> Activate(string token)
        {
            //todo : add expiration date
            /*_context.Users.First(x => SecurityUsing.Sha512(x.Token).Equals(token)).IsMailChecked = true;
            await _context.SaveChangesAsync();*/
            return View();
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
