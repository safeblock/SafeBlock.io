using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using RestSharp;
using SafeBlock.io.Models;

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

        [Route("status")]
        [Route("service-status")]
        public IActionResult Status()
        {
            try
            {
                using (var consulLookup = new WebClient())
                {
                    //TODO : putting in config file
                    dynamic consulStatus =
                        JsonConvert.DeserializeObject(
                            consulLookup.DownloadString("http://127.0.0.1:8500/v1/health/checks/safeblock"));
                    return View(consulStatus);
                }
            }
            catch
            {
                ViewBag.Error = true;
                return View();
            }
        }

        [Route("error/{errorCode?}")]
        public IActionResult Error(int errorCode = 404) 
        {
            return View(new ErrorModel() 
                { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("change-language/{lang}")]
        public IActionResult ChangeLanguage(string lang, string redirectUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            
            return LocalRedirect(redirectUrl);
        }

        [HttpPost]
        [Route("subscribe-newsletter")]
        public IActionResult SubscribeNewsletter(Newsletter newsletter)
        {
            try
            {
                //TODO : adding api key in .config file
                var client = new RestClient("https://api.sendinblue.com/v3/contacts");
                var request = new RestRequest(Method.POST);
                request.AddHeader("api-key", "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");
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefined", $"{{\"email\":\"{newsletter.Email}\",\"listIds\":[],\"smtpBlacklistSender\":[]}}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return Ok();
            }
            catch
            {
                return Error();
            }
        }
    }
    
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, 
            NoStore = true)]
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
