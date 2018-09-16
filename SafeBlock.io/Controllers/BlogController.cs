using Microsoft.AspNetCore.Mvc;

namespace SafeBlock.io.Controllers
{
    public class BlogController : Controller
    {
        [Route("blog")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("blog/{article}")]
        public IActionResult Article(string article)
        {
            return View();
        }
    }
}