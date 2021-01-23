using BookApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookApp.Controllers
{
    public class EmailNotifyController : Controller
    {
        // GET: EmailNotify
        public ActionResult Index()
        {
            try
            {

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return this.View();
        }

        public async Task<ActionResult> Index(EmailNotifyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string emailMessage = $"Dear " + model.ToMail + " ,<br/>" +
                        "<br/> This is a test <b style='color:red'> Notification <br/><br/>" +
                        " Thanks and regards. <br/>Itu Oli";

                    string emailSubj = EmaiInfo.EMAIL_SUBJECT_DEFAULT + " Test";

                    await this.SendEmailAsync(model.ToMail, emailMessage, emailSubj);

                    return this.Json(new { EnableSuccess = true, SuccessTitle = "Success", SuccessMsg = "Notification sent successful" });
                }
            }catch(Exception ex)
            {
                Console.Write(ex);
                return this.Json(new { EnableError = true, ErrorTitle = "Error", ErrorMsg=ex.Message });

            }

            return this.Json(new { EnableError = true, ErrorTitle = "Error", ErrorMsg="Something went wrong!" });
        }
        
        public async Task<bool> SendEmailAsync(string email, string msg, string subject = "")
        {
            bool isSend = false;

            try
            {
                var body = msg;
                var message = new MailMessage();

                message.To.Add(new MailAddress(email));
                message.From = new MailAddress(EmaiInfo.FROM_EMAIL_ACCOUNT);
                message.Subject = !string.IsNullOrEmpty(subject) ? subject : EmaiInfo.EMAIL_SUBJECT_DEFAULT;
                message.Body = body;
                message.IsBodyHtml = true;

                using(var smtp = new SmtpClient())
                {
                    var credentials = new NetworkCredential
                    {
                        UserName = EmaiInfo.FROM_EMAIL_ACCOUNT,
                        Password = EmaiInfo.FROM_EMAIL_PASSWORD
                    };

                    smtp.Credentials = credentials;
                    smtp.Host = EmaiInfo.SMTP_HOST_GMAIL;
                    smtp.Port = Convert.ToInt32(EmaiInfo.SMTP_PORT_GMAIL);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(message);

                    isSend = true;
                }

            }catch(Exception ex)
            {
                throw ex;
            }

            return isSend;
        }
    }
}