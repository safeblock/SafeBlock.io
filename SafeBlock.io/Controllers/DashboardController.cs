using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SafeBlock.io.Controllers
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

        [Route("dashboard/vault")]
        public IActionResult Vault()
        {
            return Content("");
        }

        [Route("dashboard/escrow")]
        public IActionResult Escrow()
        {
            return Content("");
        }

        [Route("dashboard/full-node")]
        public IActionResult FullNode()
        {
            return Content("");
        }

        [Route("dashboard/support")]
        public IActionResult Support()
        {
            return View();
        }

        [Route("dashboard/changelog")]
        public IActionResult Changelog()
        {
            return Content("");
        }

        public IActionResult MyAccount()
        {
            return Content("");
        }
    }
}