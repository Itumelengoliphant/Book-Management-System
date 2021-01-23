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
    public class InstitutionTypeController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();

        // GET: InstitutionType
        public ActionResult Index()
        {
            return View(db.InstitutionTypes.ToList());
        }

        // GET: InstitutionType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionType institutionType = db.InstitutionTypes.Find(id);
            if (institutionType == null)
            {
                return HttpNotFound();
            }
            return View(institutionType);
        }

        // GET: InstitutionType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InstitutionType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Type,Description")] InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                db.InstitutionTypes.Add(institutionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(institutionType);
        }

        // GET: InstitutionType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionType institutionType = db.InstitutionTypes.Find(id);
            if (institutionType == null)
            {
                return HttpNotFound();
            }
            return View(institutionType);
        }

        // POST: InstitutionType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Type,Description")] InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institutionType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(institutionType);
        }

        // GET: InstitutionType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InstitutionType institutionType = db.InstitutionTypes.Find(id);
            if (institutionType == null)
            {
                return HttpNotFound();
            }
            return View(institutionType);
        }

        // POST: InstitutionType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InstitutionType institutionType = db.InstitutionTypes.Find(id);
            db.InstitutionTypes.Remove(institutionType);
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
