using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BookApp.Models;

namespace BookApp.Controllers
{
    public class SellerController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();

        // GET: Seller
        public ActionResult Index()
        {
            var sellers = db.Sellers.Include(s => s.Book);
            return View(sellers.ToList());
        }

        // GET: Seller/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // GET: Seller/Create
        public ActionResult Create()
        {
            ViewBag.BookId = new SelectList(db.Books, "Id", "Title");
            return View();
        }

        // POST: Seller/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Surname,Phone,Email,Subject,Enquiry,Address,BookId,AllowSeller")] Seller seller)
        {
            if (ModelState.IsValid)
            {

                string emailMessage = $"Dear " + seller.Email + " ,<br/>" +
                        "<br/> This is email servers to inform you that user : <br/><br/>" +"<b>"+ seller.Name+"<b/>" + " " + seller.Surname
                       + "<br/> Phone number: "+seller.Phone + "<br/> might be having an interest in purchasing your book<br/><br/>" +
                       "" +
                       "The user left this message below:<br/><br/>" + "<i>"+ seller.Enquiry +"<i/>"+ "<br/><br/>Kind regards</br><br/><b>Book App 2020 Team<b/>";

                string emailSubj = EmaiInfo.EMAIL_SUBJECT_DEFAULT;

                await this.SendEmailAsync(seller.Email, emailMessage, emailSubj);

               // return this.Json(new { EnableSuccess = true, SuccessTitle = "Success", SuccessMsg = "Notification sent successful" });

                db.Sellers.Add(seller);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookId = new SelectList(db.Books, "Id", "Title", seller.BookId);
            return View(seller);
        }

        // GET: Seller/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookId = new SelectList(db.Books, "Id", "Title", seller.BookId);
            return View(seller);
        }

        // POST: Seller/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Phone,Email,Subject,Enquiry,Address,BookId,AllowSeller")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookId = new SelectList(db.Books, "Id", "Title", seller.BookId);
            return View(seller);
        }

        // GET: Seller/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Seller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seller seller = db.Sellers.Find(id);
            db.Sellers.Remove(seller);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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

                using (var smtp = new SmtpClient())
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

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSend;
        }

    }
}
