using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;

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

        [Route("service-status")]
        public IActionResult Status()
        {
            return View();
        }

        [Route("error/{errorCode?}")]
        public IActionResult Error(int errorCode = 404) 
        {
            return View(errorCode);
        }

        [Route("change-language/{lang}")]
        public IActionResult ChangeLanguage(string lang, string redirectUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)), new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)});
            
            return LocalRedirect(redirectUrl);
        }

        [HttpPost]
        [Route("subscribe-newsletter")]
        public IActionResult SubscribeNewsletter(string email)
        {
            return Ok();
        }
    }
}
