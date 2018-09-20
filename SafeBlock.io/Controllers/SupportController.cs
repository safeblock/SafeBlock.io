using Microsoft.AspNetCore.Mvc;
using SafeBlock.io.Models;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Microsoft.Extensions.Options;
using SafeBlock.io.Settings;
using System.Net.Mail;

namespace SafeBlock.io.Controllers
{
    public class SupportController : Controller
    {
        private readonly IOptions<MailingSettings> _mailingSettings;
        private readonly ISupport _support;

        public SupportController(IOptions<MailingSettings> mailingSettings, ISupport support)
        {
            _mailingSettings = mailingSettings;
            _support = support;
        }

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
                return RedirectToAction("ViewCategory", new {category=knowledgeBase.Query});
            }
            
            return View("ViewCategory", knowledgeBase);
        }

        [Route("support/search/{term}")]
        public IActionResult ViewCategory(string term)
        {
            return View(new KnowledgeBase()
            {
                Query = term,
                Items =  _support.GetArticlesByTerm(term)
            });
        }

        [Route("support/read/{article}")]
        public IActionResult ReadArticle(string article)
        {
            var supportArticle = _support.GetArticleBySeo(article);
            return View(supportArticle);
        }

        [Route("support/send-message")]
        [ValidateRecaptcha]
        public IActionResult SendMessage(ContactDatas contactDatas)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var SmtpServer = new SmtpClient(_mailingSettings.Value.SMTP)
                    {
                        Port = 587,
                        Credentials = new System.Net.NetworkCredential(_mailingSettings.Value.Mail, _mailingSettings.Value.Password),
                        EnableSsl = true
                    };
                    var mail = new MailMessage()
                    {
                        From = new MailAddress(_mailingSettings.Value.Mail),
                        ReplyToList = { contactDatas.Email },
                        Subject = "[Auto] Message from SafeBlock.io",
                        Body = "From: " + contactDatas.Email + "\n" + "Subject: " + contactDatas.Subject + "\nContent: " + contactDatas.Content
                    };
                    mail.To.Add(_mailingSettings.Value.Mail);

                    SmtpServer.Send(mail);

                    return new OkResult();
                }
                catch
                {
                    return NotFound();
                }
            }
            return NotFound();
        }
    }
}