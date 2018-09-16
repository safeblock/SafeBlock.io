using Microsoft.AspNetCore.Mvc;
using SafeBlock.io.Models;

namespace SafeBlock.io.Controllers
{
    public class SupportController : Controller
    {
        [Route("support")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("support/search")]
        public IActionResult SearchKnowledgebase(KnowledgeBase knowledgeBase)
        {
            if (ModelState.IsValid)
            {
            }
            
            return View("ViewCategory", knowledgeBase);
        }

        [Route("support/search/{category}")]
        public IActionResult ViewCategory(string category)
        {
            return View(new KnowledgeBase()
            {
                Query = category
            });
        }

        [Route("support/read/{article}")]
        public IActionResult ReadArticle(string article)
        {
            return View();
        }

        public IActionResult SendMessage()
        {
            return Content("salut");
        }
    }
}