using BookApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;


namespace BookApp.Controllers
{
    public class InstitutionController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();

        // GET: Institution
        public ActionResult Index(string option, string search,int? pageNumber)
        {

            var institutions = db.Institutions.Include(i => i.InstitutionType);

            if (option == "Name")
            {
                return View(db.Institutions.Where(x => x.Name.Contains(search) || search == null).ToList().ToPagedList(pageNumber ?? 1, 10));
            }
            else
            {
                int instId = Convert.ToInt32(search);

                return View(db.Institutions.Where(x => x.Id == instId || search == null).ToList().ToPagedList(pageNumber ?? 1, 10));
            }
        }

        // GET: Institution/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // GET: Institution/Create
        public ActionResult Create()
        {
            ViewBag.TypeId = new SelectList(db.InstitutionTypes, "id", "Type");
            return View();
        }

        // POST: Institution/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,TypeId")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                db.Institutions.Add(institution);
                db.SaveChanges();
                TempData["successMessage"] = $"Institution successfully Added";
                return RedirectToAction("Index");
            }

            ViewBag.TypeId = new SelectList(db.InstitutionTypes, "id", "Type", institution.TypeId);
            return View(institution);
        }

        // GET: Institution/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeId = new SelectList(db.InstitutionTypes, "id", "Type", institution.TypeId);
            return View(institution);
        }

        // POST: Institution/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,TypeId")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                db.Entry(institution).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeId = new SelectList(db.InstitutionTypes, "id", "Type", institution.TypeId);
            return View(institution);
        }

        // GET: Institution/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: Institution/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Institution institution = db.Institutions.Find(id);
            db.Institutions.Remove(institution);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RemoveInstitution(int id)
        {

            Institution institution = db.Institutions.Find(id);
            db.Institutions.Remove(institution);
            db.SaveChanges();
            TempData["successMessage"] = $"Institution successfully deleted";
            return RedirectToAction("Index");
        }


        public JsonResult GetSearchResults(string SearchBy, string SearchValue)
        {
            List<Institution> list = new List<Institution>();

            if (!SearchBy.Equals("Name"))
            {
                list = db.Institutions.Where(x => x.Name.Contains(SearchValue) || SearchValue == null).ToList();
            }
            else
            {
                list = db.Institutions.Where(x => x.Name.Contains(SearchValue) || SearchValue == null).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);

            }
            return Json(list, JsonRequestBehavior.AllowGet);

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
