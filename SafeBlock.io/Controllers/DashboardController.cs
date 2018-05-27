using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SafeBlock.Io.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [Route("dashboard")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("dashboard/choose-wallet")]
        public IActionResult ChooseWallet()
        {
            return View();
        }

        [Route("dashboard/security")]
        public IActionResult Security()
        {
            return View();
        }

        [Route("dashboard/exchange")]
        public IActionResult Exchange()
        {
            return View();
        }

        [Route("dashboard/support")]
        public IActionResult Support()
        {
            return View();
        }

        public IActionResult MyAccount()
        {
        }
    }
}