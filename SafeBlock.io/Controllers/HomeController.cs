using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeBlock.Io.Models;
using SafeBlock.Io.Services;
using VaultSharp;
using VaultSharp.Backends.Authentication.Models.AppId;
using VaultSharp.Backends.Authentication.Models.Custom;
using VaultSharp.Backends.Authentication.Models.Token;

namespace SafeBlock.Io.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Route("change-language/{lang}")]
        public IActionResult ChangeLanguage(string lang)
        {
            //TODO : adding localization
            return RedirectToAction("Index");
        }
        
        [Route("privacy-policy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }
        
        [Route("services")]
        public IActionResult Services()
        {
            return View();
        }
        
        [Route("support")]
        public IActionResult Support()
        {
            return View();
        }

        [Route("support/search")]
        public IActionResult SearchKnowledgebase(KnowledgeBase knowledgeBase)
        {
            if (ModelState.IsValid)
            {
                
            }
            
            return View("Support", knowledgeBase);
        }

        [Route("redirect/tor-service")]
        public async Task<IActionResult> VisitTor()
        {
            using (var torCheck = new WebClient())
            {
                //TODO : put in config file
                const string ip = "207.246.82.118";
                if (torCheck.DownloadString(
                    $"https://check.torproject.org/cgi-bin/TorBulkExitList.py?ip={ip}&port=").Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    return Redirect("http://safeblock3b4qmoqbpl.onion");
                }
            }
            
            return RedirectToAction("TorFriendly");
        }

        [Route("tor-friendly")]
        public IActionResult TorFriendly()
        {
            return View();
        }
    }
}
