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

        /*

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
            return View();
        }

        [Route("dashboard/escrow")]
        public IActionResult Escrow()
        {
            return View();
        }

        [Route("dashboard/full-node")]
        public IActionResult FullNode()
        {
            return View();
        }

        [Route("dashboard/support")]
        public IActionResult Support()
        {
            return View();
        }

        [Route("dashboard/version")]
        public IActionResult Version()
        {
            return View();
        }

        public IActionResult MyAccount()
        {
            return Content("");
        }*/
    }
}