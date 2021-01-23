using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookApp.Models;
using Microsoft.Reporting.WebForms;

namespace BookApp.Controllers
{
    public class BookController : Controller
    {
        private BookAppDbEntities db = new BookAppDbEntities();
        List<ShoppingCartModel> listOfShoppingCartModels;

        public BookController()
        {
            listOfShoppingCartModels = new List<ShoppingCartModel>();
        }

        // GET: Book
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Category).Include(b => b.Condition).Include(b => b.Institution);
            return View(books.ToList());
        }

        // GET: Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1");
            ViewBag.ConditionId = new SelectList(db.Conditions, "Id", "Condition1");
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book)
        {
                string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                string extension = Path.GetExtension(book.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                book.Image = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                book.ImageFile.SaveAs(fileName);

            using (BookAppDbEntities bookDb = new BookAppDbEntities())
            {

                if (ModelState.IsValid)
                {
                    db.Books.Add(book);
                    TempData["successMessage"] = $"Book successfully added";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(" ", "Error: Please fill in all the fields!");
                }
            }


            ModelState.Clear();

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1", book.CategoryId);
            ViewBag.ConditionId = new SelectList(db.Conditions, "Id", "Condition1", book.ConditionId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", book.InstitutionId);
            return View(book);
        }

        // GET: Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1", book.CategoryId);
            ViewBag.ConditionId = new SelectList(db.Conditions, "Id", "Condition1", book.ConditionId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", book.InstitutionId);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,CategoryId,InstitutionId,ISBN,Author,Edition,Publisher,ModuleCode,ConditionId,Price,Description,Image")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Category1", book.CategoryId);
            ViewBag.ConditionId = new SelectList(db.Conditions, "Id", "Condition1", book.ConditionId);
            ViewBag.InstitutionId = new SelectList(db.Institutions, "Id", "Name", book.InstitutionId);
            return View(book);
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RemoveBook(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            TempData["successMessage"] = $"Book successfully deleted";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Index(string ItemId)
        {
            ShoppingCartModel cartModel = new ShoppingCartModel();
            Book bookItem = db.Books.Single(model => model.Id.ToString() == ItemId);
            if (Session["CartCounter"] != null)
            {
                listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            }
            if(listOfShoppingCartModels.Any(model => model.ItemId == ItemId))
            {
                cartModel = listOfShoppingCartModels.Single(x => x.ItemId == ItemId);
                cartModel.Quantity = cartModel.Quantity + 1;
                cartModel.Total = Convert.ToInt32(cartModel.Quantity * cartModel.UnitPrice);
            }
            else
            {
                cartModel.ItemId = ItemId;
                cartModel.ImagePath = bookItem.Image;
                cartModel.ItemName = bookItem.Title;
                cartModel.Quantity = 1;
                cartModel.UnitPrice = bookItem.Price;
                //Add more properties based on this logic

                listOfShoppingCartModels.Add(cartModel);
            }

            Session["CartCounter"] = listOfShoppingCartModels.Count;
            Session["CartItem"] = listOfShoppingCartModels;
            return Json(new { Success = true, Counter = listOfShoppingCartModels.Count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BookShoppingCart()
        {
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;

            return View(listOfShoppingCartModels);
        }

        [HttpPost]
        public ActionResult AddOrder()
        {
            int orderId = 0;
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            Order order = new Order()
            {
                OrderDate = DateTime.Now,
                OrderNumber = String.Format("{0:ddmmyyyyHHmmsss}",DateTime.Now)
            };

            db.Orders.Add(order);
            db.SaveChanges();
            orderId = order.OrderId;

            foreach(var items in listOfShoppingCartModels)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Total = items.Total;
                orderDetail.ItemId = items.ItemId;
                orderDetail.OrderId = orderId;
                orderDetail.Quantity = items.Quantity;
                orderDetail.UnitPrice = items.UnitPrice;
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }

            Session["CartItem"] = null;
            Session["CartCounter"] = null;
            return RedirectToAction("Index");

        }

        public ActionResult Reports(string ReportType)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/BookReport.rdlc");

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "BookDataSet";
            reportDataSource.Value = db.Books.ToList();
            localReport.DataSources.Add(reportDataSource);

            string reportType = ReportType;
            string mimeTime;
            string encoding;
            string fileNameExtension;

            if (reportType == "Word")
            {
                fileNameExtension = "docx";
            }

            else if (reportType == "PDF")
            {
                fileNameExtension = "pdf";
            }

            string[] streams;
            Warning[] warnings;
            byte[] renderdByte;

            renderdByte = localReport.Render(reportType, "", out mimeTime, out encoding, out fileNameExtension, out streams, out warnings);

            Response.AddHeader("content-disposition", "attachment;fileName=BookReport." + fileNameExtension);
            return File(renderdByte, fileNameExtension);
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
