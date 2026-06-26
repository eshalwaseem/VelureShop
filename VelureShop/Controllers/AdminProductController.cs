using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models;
using System.IO;

namespace VelureShop.Controllers
{
    public class AdminProductController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        private bool IsAdmin()
        {
            return Session["UserID"] != null && Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // GET: /AdminProduct
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var products = db.Products.Include(p => p.Category).OrderByDescending(p => p.ProductID).ToList();
            return View(products);
        }

        // GET: /AdminProduct/Create
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(c => c.Gender).ThenBy(c => c.Name), "CategoryID", "Name");
            return View();
        }

        // POST: /AdminProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);
                string savePath = Server.MapPath("~/Images/products/" + fileName);
                ImageFile.SaveAs(savePath);
                product.ImagePath = fileName;
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        // GET: /AdminProduct/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.CategoryID = new SelectList(db.Categories.OrderBy(c => c.Gender).ThenBy(c => c.Name), "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        // POST: /AdminProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", product.CategoryID);
            return View(product);
        }

        // GET: /AdminProduct/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: /AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = db.Products.Find(id);
            if (product != null)
            {
                // soft delete keeps order history intact
                product.IsActive = false;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
