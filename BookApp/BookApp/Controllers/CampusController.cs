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
    public class CampusController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();

        // GET: Campus
        public ActionResult Index()
        {
            var campus1 = db.Campus1.Include(c => c.Institution);
            return View(campus1.ToList());
        }

        // GET: Campus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campus campus = db.Campus1.Find(id);
            if (campus == null)
            {
                return HttpNotFound();
            }
            return View(campus);
        }

        // GET: Campus/Create
        public ActionResult Create()
        {
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name");
            return View();
        }

        // POST: Campus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Email,InstitutionId")] Campus campus)
        {
            if (ModelState.IsValid)
            {
                db.Campus1.Add(campus);
                TempData["successMessage"] = $"Campus successfully added";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", campus.InstitutionId);
            return View(campus);
        }

        // GET: Campus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campus campus = db.Campus1.Find(id);
            if (campus == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", campus.InstitutionId);
            return View(campus);
        }

        // POST: Campus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Email,InstitutionId")] Campus campus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campus).State = EntityState.Modified;
                db.SaveChanges();
                TempData["successMessage"] = $"Campus successfully Updated";
                return RedirectToAction("Index");
            }
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", campus.InstitutionId);
            return View(campus);
        }

        // GET: Campus/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Campus campus = db.Campus1.Find(id);
        //    if (campus == null)
        //    {
        //        return HttpNotFound();
        //    }  
        //    return View(campus);
        //}

        // POST: Campus/Delete/5

        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Campus campus = db.Campus1.Find(id);
            db.Campus1.Remove(campus);
            db.SaveChanges();
            TempData["successMessage"] = $"Record successfully deleted";
            return RedirectToAction("Index");
        }

        public ActionResult RemoveItem(int id)
        {
            Campus campus = db.Campus1.Find(id);
            db.Campus1.Remove(campus);
            db.SaveChanges();
            TempData["successMessage"] = $"Record successfully deleted";
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
