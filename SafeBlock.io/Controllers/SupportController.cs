using Microsoft.AspNetCore.Mvc;
using SafeBlock.Io.Models;

namespace SafeBlock.Io.Controllers
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
            
            return View("Index", knowledgeBase);
        }
    }
}