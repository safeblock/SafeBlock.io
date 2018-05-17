using Microsoft.AspNetCore.Mvc;

namespace SafeBlock.Io.Controllers
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