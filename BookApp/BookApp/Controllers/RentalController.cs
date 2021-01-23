using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookApp.Models;

namespace BookApp.Controllers
{
    public class RentalController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();

        // GET: Rental
        public ActionResult Index()
        {
            var rentals = db.Rentals.Include(r => r.Book).Include(r => r.User);
            return View(rentals.ToList());
        }

        // GET: Rental/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // GET: Rental/Create
        public ActionResult Create()
        {
            ViewBag.BookId = new SelectList(db.Books, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName");
            return View();
        }

        // POST: Rental/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,BookId,GuardianName,GuardianSurname,GuardianID,GuardianCell,GuardianAddress,HasPenalty,DateOfRent,DateOfReturn,ExpectedDate")] Rental rental)
        {
            var rented = db.Rentals.Where(x => x.BookId == rental.BookId).FirstOrDefault();

            ViewBag.BookId = new SelectList(db.Books, "Id", "Title", rental.BookId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", rental.UserId);

            if (rented!=null)
            {
                ViewBag.SetMessage = $"Sorry book has already been rented, from {rented.DateOfRent} till {rented.DateOfReturn}";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Rentals.Add(rental);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

               
                return View(rental);
            }

           
            
        }

        // GET: Rental/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookId = new SelectList(db.Books, "Id", "Title", rental.BookId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", rental.UserId);
            return View(rental);
        }

        // POST: Rental/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,BookId,GuardianName,GuardianSurname,GuardianID,GuardianCell,GuardianAddress,HasPenalty,DateOfRent,DateOfReturn,ExpectedDate")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookId = new SelectList(db.Books, "Id", "Title", rental.BookId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", rental.UserId);
            return View(rental);
        }

        // GET: Rental/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rental/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rental rental = db.Rentals.Find(id);
            db.Rentals.Remove(rental);
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
    }
}
