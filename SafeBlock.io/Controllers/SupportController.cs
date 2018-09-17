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
        private IOptions<MailingSettings> _mailingSettings;

        public SupportController(IOptions<MailingSettings> mailingSettings)
        {
            _mailingSettings = mailingSettings;
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
            return Content("salut");
        }
    }
}