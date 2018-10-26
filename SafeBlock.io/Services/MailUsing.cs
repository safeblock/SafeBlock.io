using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using SafeBlock.io.Settings;

namespace SafeBlock.io.Services
{
    public static class MailUsing
    {
        private static IConfigurationRoot Configuration { get; set; }
        
        public static async Task<bool> SendConfirmationEmail(string emailAddress, string activationLink, string certificateFile)
        {
            //TODO: create logs of SendInBlue Response
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            
            var client = new RestClient("https://api.sendinblue.com/v3/smtp/email");
            var sendInBlueRequest = new RestRequest(Method.POST);
            sendInBlueRequest.AddHeader("api-key", Configuration.GetSection("SendInBlue").GetValue<string>("ApiKey"));
            sendInBlueRequest.AddHeader("Accept", "application/json");
            sendInBlueRequest.AddHeader("Content-Type", "application/json");
            sendInBlueRequest.AddParameter("undefined", $"{{\"tags\":[\"Activate account for {{emailAddress}}\"],\"sender\":{{\"email\":\"contact@safeblock.io\",\"name\":\"SafeBlock.io\"}},\"htmlContent\":\"\",\"replyTo\":{{\"email\":\"contact@safeblock.io\",\"name\":\"SafeBlock.io Support\"}},\"templateId\":2,\"to\":[{{\"email\":\"{emailAddress}\",\"name\":\"{emailAddress}\"}}],\"params\":{{\"ActivationLink\":\"{activationLink}\"}}}}", ParameterType.RequestBody);
            var sendInBlueResponse = await client.ExecuteTaskAsync(sendInBlueRequest);
            return sendInBlueResponse.IsSuccessful;
        }

        /// Verifie si le mail est valide ou pas
        public static bool IsValidMail(string mail)
        {
            return new EmailAddressAttribute().IsValid(mail);
        }

        /// Verifie si le mail fait partie d'un domaine banni
        public static bool IsBannedMail(string mail)
        {
            List<string> BannedDomains = new List<string>() { "0815.ru0clickemail.com", "0-mail.com", "0wnd.net", "0wnd.org", "10minutemail.com", "20minutemail.com", "2prong.com", "3d-painting.com", "4warding.com", "4warding.net", "4warding.org", "9ox.net", "a-bc.net", "ag.us.to", "amilegit.com", "anonbox.net", "anonymbox.com", "antichef.com", "antichef.net", "antispam.de", "baxomale.ht.cx", "beefmilk.com", "binkmail.com", "bio-muesli.net", "bobmail.info", "bodhi.lawlita.com", "bofthew.com", "brefmail.com", "bsnow.net", "bugmenot.com", "bumpymail.com", "casualdx.com", "chogmail.com", "cool.fr.nf", "correo.blogos.net", "cosmorph.com", "courriel.fr.nf", "courrieltemporaire.com", "curryworld.de", "cust.in", "dacoolest.com", "dandikmail.com", "deadaddress.com", "despam.it", "despam.it", "devnullmail.com", "dfgh.net", "digitalsanctuary.com", "discardmail.com", "discardmail.de", "disposableaddress.com", "disposeamail.com", "disposemail.com", "dispostable.com", "dm.w3internet.co.ukexample.com", "dodgeit.com", "dodgit.com", "dodgit.org", "dontreg.com", "dontsendmespam.de", "dump-email.info", "dumpyemail.com", "e4ward.com", "email60.com", "emailias.com", "emailias.com", "emailinfive.com", "emailmiser.com", "emailtemporario.com.br", "emailwarden.com", "enterto.com", "ephemail.net", "explodemail.com", "fakeinbox.com", "fakeinformation.com", "fansworldwide.de", "fastacura.com", "filzmail.com", "fixmail.tk", "fizmail.com", "frapmail.com", "garliclife.com", "gelitik.in", "get1mail.com", "getonemail.com", "getonemail.net", "girlsundertheinfluence.com", "gishpuppy.com", "goemailgo.com", "great-host.in", "greensloth.com", "greensloth.com", "gsrv.co.uk", "guerillamail.biz", "guerillamail.com", "guerillamail.net", "guerillamail.org", "guerrillamail.biz", "guerrillamail.com", "guerrillamail.de", "guerrillamail.net", "guerrillamail.org", "guerrillamailblock.com", "haltospam.com", "hidzz.com", "hotpop.com", "ieatspam.eu", "ieatspam.info", "ihateyoualot.info", "imails.info", "inboxclean.com", "inboxclean.org", "incognitomail.com", "incognitomail.net", "ipoo.org", "irish2me.com", "jetable.com", "jetable.fr.nf", "jetable.net", "jetable.org", "jnxjn.com", "junk1e.com", "kasmail.com", "kaspop.com", "klzlk.com", "kulturbetrieb.info", "kurzepost.de", "kurzepost.de", "lifebyfood.com", "link2mail.net", "litedrop.com", "lookugly.com", "lopl.co.cc", "lr78.com", "maboard.com", "mail.by", "mail.mezimages.net", "mail4trash.com", "mailbidon.com", "mailcatch.com", "maileater.com", "mailexpire.com", "mailin8r.com", "mailinator.com", "mailinator.net", "mailinator2.com", "mailincubator.com", "mailme.lv", "mailmetrash.com", "mailmoat.com", "mailnator.com", "mailnull.com", "mailzilla.org", "mbx.cc", "mega.zik.dj", "meltmail.com", "mierdamail.com", "mintemail.com", "mjukglass.nu", "mobi.web.id", "moburl.com", "moncourrier.fr.nf", "monemail.fr.nf", "monmail.fr.nf", "mt2009.com", "mx0.wwwnew.eu", "mycleaninbox.net", "myspamless.com", "mytempemail.com", "mytrashmail.com", "netmails.net", "neverbox.com", "no-spam.ws", "nobulk.com", "noclickemail.com", "nogmailspam.info", "nomail.xl.cx", "nomail2me.com", "nospam.ze.tc", "nospam4.us", "nospamfor.us", "nowmymail.com", "objectmail.com", "obobbo.com", "odaymail.com", "onewaymail.com", "ordinaryamerican.net", "owlpic.com", "pookmail.com", "privymail.de", "proxymail.eu", "punkass.com", "putthisinyourspamdatabase.com", "quickinbox.com", "rcpt.at", "recode.me", "recursor.net", "regbypass.comsafe-mail.net", "safetymail.info", "sandelf.de", "saynotospams.com", "selfdestructingmail.com", "sendspamhere.com", "sharklasers.com", "shieldedmail.com", "shiftmail.com", "skeefmail.com", "slopsbox.com", "slushmail.com", "smaakt.naar.gravel", "smellfear.com", "snakemail.com", "sneakemail.com", "sofort-mail.de", "sogetthis.com", "soodonims.com", "spam.la", "spamavert.com", "spambob.net", "spambob.org", "spambog.com", "spambog.de", "spambog.ru", "spambox.info", "spambox.us", "spamcannon.com", "spamcannon.net", "spamcero.com", "spamcorptastic.com", "spamcowboy.com", "spamcowboy.net", "spamcowboy.org", "spamday.com", "spamex.com", "spamfree.eu", "spamfree24.com", "spamfree24.de", "spamfree24.eu", "spamfree24.info", "spamfree24.net", "spamfree24.org", "spamgourmet.com", "spamgourmet.net", "spamgourmet.org", "spamherelots.com", "spamhereplease.com", "spamhole.com", "spamify.com", "spaminator.de", "spamkill.info", "spaml.com", "spaml.de", "spammotel.com", "spamobox.com", "spamspot.com", "spamthis.co.uk", "spamthisplease.com", "speed.1s.fr", "suremail.info", "tempalias.com", "tempe-mail.com", "tempemail.biz", "tempemail.com", "tempemail.net", "tempinbox.co.uk", "tempinbox.com", "tempomail.fr", "temporaryemail.net", "temporaryinbox.com", "tempymail.com", "thankyou2010.com", "thisisnotmyrealemail.com", "throwawayemailaddress.com", "tilien.com", "tmailinator.com", "tradermail.info", "trash-amil.com", "trash-mail.at", "trash-mail.com", "trash-mail.de", "trash2009.com", "trashmail.at", "trashmail.com", "trashmail.me", "trashmail.net", "trashmailer.com", "trashymail.com", "trashymail.net", "trillianpro.com", "tyldd.com", "tyldd.com", "uggsrock.com", "wegwerfmail.de", "wegwerfmail.net", "wegwerfmail.org", "wh4f.org", "whyspam.me", "willselfdestruct.com", "winemaven.info", "wronghead.com", "wuzupmail.net", "xoxy.net", "yogamaven.com", "yopmail.com", "yopmail.fr", "yopmail.net", "yuurok.com", "zippymail.info", "zoemail.com" };

            if(BannedDomains.Contains(mail.ToLower().Trim().Split('@')[1]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}