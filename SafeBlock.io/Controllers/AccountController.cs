using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SafeBlock.Io.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SafeBlock.Io.Services;
using SafeBlock.Io.Settings;
using Vault;

namespace SafeBlock.Io.Controllers
{
    public class AccountController : Controller
    {
        private SafeBlockContext _context;
        private IHostingEnvironment _env;
        private VaultClient _vaultClient;

        public AccountController(IHostingEnvironment env, IOptions<VaultSettings> _vaultSettings, SafeBlockContext context)
        {
            _env = env;
            _context = context;
            
            _vaultClient = new VaultClient()
            {
                Address = new Uri(_vaultSettings.Value.ConnectionString),
                Token = _vaultSettings.Value.Token
            };
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
                    
                    var newUser = new Users
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
                    sb.Users.Add(newUser);
                    try
                    {
                        sb.SaveChanges();
                        
                        //TODO : store in secret
                        
                        // Chiffrement du token
                        var firstCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(
                            "8a174f91ebc1713f62108712267eca28dcf8bcc12d155c9dd79cd30661a7a1d665350330c074cd9cd9c702ba7e750192188aca5fefbb942e822862da9c4c7dba",
                            SecurityToken));
                        var secondCrypt = SecurityUsing.BytesToHex(Aes.Encrypt(
                            handleLoginModel.Password,
                            firstCrypt));
                        
                        // Création du token dans le vault (double chiffré)
                        await _vaultClient.Secret.Write(Path.Combine("secret", "accounts", SecurityUsing.Sha1(handleLoginModel.Mail.ToLower())), new Dictionary<string, string>
                        {
                            {"token", secondCrypt}
                        });
                        
                        // Connexion de l'utilisateur 
                        SignInUser(handleLoginModel.Mail);
                        
                        //TODO : send mail if it's good
                        // Envoi du mail de confirmation
                        MailUsing.SendConfirmationMail(handleLoginModel.Mail, Path.Combine(_env.ContentRootPath, "Datas", "CreateAccountMail.html"), @"F:\SafeBlock.io\Backup\unx\SafeBlock.Io\robots.txt");
                        
                        return RedirectToAction("ChooseWallet", "Dashboard");
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError("Mail", "This mail address is already used.");
                    }
                }
            }
            
            return View("GettingStarted", handleLoginModel);
        }

        // Routine de connexion
        private void SignInUser(string mail, string role = "user", bool persistent = false)
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
        public IActionResult Login(HandleLoginModel handleLoginModel)
        {
            ViewBag.Section = "login";
            ModelState.Remove("VerifyPassword");
            
            if (ModelState.IsValid)
            {
                var userPresence = _context.Users.Where(x => x.Mail.Equals(handleLoginModel.Mail.ToLower()));
                if (userPresence.Any())
                {
                    try
                    {
                        var fullyCryptedToken = _vaultClient.Secret.Read<Dictionary<string, string>>(Path.Combine("secret", "accounts", SecurityUsing.Sha1(handleLoginModel.Mail.ToLower())));
                        var halfCryptedToken = Aes.Decrypt(handleLoginModel.Password, SecurityUsing.HexToBytes(fullyCryptedToken.Result.Data.First(x => x.Key == "token").Value));
                        var token = Aes.Decrypt("8a174f91ebc1713f62108712267eca28dcf8bcc12d155c9dd79cd30661a7a1d665350330c074cd9cd9c702ba7e750192188aca5fefbb942e822862da9c4c7dba", SecurityUsing.HexToBytes(halfCryptedToken));

                        var user = userPresence.First();
                        if (token.ToLower().Equals(user.Token))
                        {
                            SignInUser(user.Mail, user.Role, handleLoginModel.KeepSession);

                            return RedirectToAction("Index", "Dashboard");
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("Mail", "Unable to decrypt your account.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Mail", "This account does not exists.");   
                }
            }
            return View("GettingStarted", handleLoginModel);
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