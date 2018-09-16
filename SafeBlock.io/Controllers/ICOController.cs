using Microsoft.AspNetCore.Mvc;

namespace SafeBlock.io.Controllers
{
    public class ICOController : Controller
    {
        [Route("ico")]
        public IActionResult Index()
        {
            return View();
        }
    }
}