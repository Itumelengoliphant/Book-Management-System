using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookApp.Models;

namespace BookApp.Controllers
{
    
    public class AuthenticationController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();
        public ActionResult Index()
        {
            return View();
        }

      
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
               // var password = GetMD5(password);
                var data = db.Users.Where(m => m.Email.Equals(email) && m.Password.Equals(password));

                if(data.Count() > 0)
                {
                    Session["username"] = data.FirstOrDefault().Email;
                    return RedirectToAction("Index");

                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");

                }
            }
            return View();
        }
    }
}