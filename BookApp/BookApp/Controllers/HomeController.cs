using BookApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookApp.Controllers
{
    public class HomeController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();

        public ActionResult Index()
        {
            var users = db.Users;
            return View(users.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            if(!string.IsNullOrEmpty(login.EmailID))
            {
                if (!string.IsNullOrEmpty(login.Password))
                {
                    if (ModelState.IsValid)
                    {
                        string message = "";

                        using (BookAppDbEntities dc = new BookAppDbEntities())
                        {
                            var getUser = dc.Users.Where(a => a.Email == login.EmailID).FirstOrDefault();
                            Session["BookCount"] = dc.Books.ToList().Count();

                            if (getUser != null)
                            {
                                if (!getUser.IsEmailVerified)
                                {
                                    ViewBag.Message = "Please verify your email first";
                                    return View();
                                }

                                if (string.Compare(Crypto.Hash(login.Password), getUser.Password) == 0)
                                {
                                    int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                                    var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                                    string encrypted = FormsAuthentication.Encrypt(ticket);
                                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                                    cookie.HttpOnly = true;
                                    Response.Cookies.Add(cookie);


                                    if (Url.IsLocalUrl(ReturnUrl))
                                    {
                                        return Redirect(ReturnUrl);
                                    }
                                    else
                                    {
                                        Session["FirstName"] = getUser.FirstName;
                                        Session["LastName"] = getUser.LastName;
                                        Session["ID"] = getUser.ID_Number;
                                        Session["Address"] = getUser.Address;
                                        Session["Phone"] = getUser.Phone;
                                        Session["Email"] = getUser.Email;


                                        return RedirectToAction("WelcomePage", "Home");

                                    }
                                }
                                else
                                {
                                    message = "Invalid credential provided";
                                }
                            }
                            else
                            {
                                message = "Invalid credential provided";
                            }
                        }
                        ViewBag.Message = message;
                        return View();
                    }

                }
                else
                {
                    ViewBag.Message = "Error: Password is a required field!";
                }
            }
            else
            {
                ViewBag.Message = "Error: Username/Email is a required field!";

            }

            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Registration()
        {
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Gender1");
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";

            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Gender1", user.GenderId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", user.InstitutionId);


            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = DoesEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View();

                }


                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                #endregion
                user.IsEmailVerified = false;

                #region Save to Database
                using (BookAppDbEntities db = new BookAppDbEntities())
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                    message = " Registration successfully done. Account activation link " +
                        " has been sent to your email: " + user.Email + "\nKindly click on the link to activate your account.\n" +
                        " BookApp2020";
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;

            return View(user);
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            string message = "";
            bool status = false;

            if (string.IsNullOrEmpty(email))
            {
                message = "Error: Email is a required field!";
            }
            else
            {
                using (BookAppDbEntities db = new BookAppDbEntities())
                {
                    var account = db.Users.Where(a => a.Email == email).FirstOrDefault();
                    if (account != null)
                    {
                        string resetCode = Guid.NewGuid().ToString();
                        SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                        account.ResetPasswordCode = resetCode;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "Reset password link has been sent to your email id.";
                    }
                    else
                    {
                        message = "Account not found";
                    }
                }

            }

            ViewBag.Message = message;
            return View();
        }

        [NonAction]
        public bool DoesEmailExist(string email)
        {
            using (BookAppDbEntities dc = new BookAppDbEntities())
            {
                var getMail = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                return getMail != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Home/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress(EmaiInfo.FROM_EMAIL_ACCOUNT, "Book App2021");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = EmaiInfo.FROM_EMAIL_PASSWORD;

            string subject = "";
            string body = "";

            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";

                body = "<br/><br/>We are excited to tell you that your Book-App2021 account is" +
                   " successfully created. Please click on the below link to verify your account" +
                   " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,</br></br>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }

            var smtp = new SmtpClient
            {
                Host = EmaiInfo.SMTP_HOST_GMAIL,
                Port = Convert.ToInt32(EmaiInfo.SMTP_PORT_GMAIL),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

                smtp.Send(message);
        }
        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (BookAppDbEntities db = new BookAppDbEntities())
            {
                var user = db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (BookAppDbEntities dc = new BookAppDbEntities())
                {
                    var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult WelcomePage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (BookAppDbEntities dc = new BookAppDbEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session["FirstName"] = null;
            Session["LastName"] = null;
            Session["ID"] = null;
            Session["Address"] = null;
            Session["Phone"] = null;
            Session["Email"] = null;

            //return RedirectToAction("Login", "Home");
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Gender1", user.GenderId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", user.InstitutionId);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FirstName,LastName,Email,ID_Number,Address,Phone,GenderId,InstitutionId,Password,IsEmailVerified,ActivationCode,ForrgotPasswordCode")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "Gender1", user.GenderId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", user.InstitutionId);
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



    }
}
