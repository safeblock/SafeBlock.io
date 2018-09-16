using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace SafeBlock.io.Controllers
{
    public class HomeController : Controller
    {
        private IStringLocalizer<HomeController> _localizer;

        public HomeController(IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewBag.Test = _localizer["Test"];
            return View();
        }

        [Route("error/{errorCode?}")]
        public IActionResult Error(int errorCode = 404) 
        {
            return View(errorCode);
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
