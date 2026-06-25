using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models;

namespace VelureShop.Controllers
{
    public class AdminCategoryController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        private bool IsAdmin()
        {
            return Session["UserID"] != null && Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // GET: /AdminCategory
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var categories = db.Categories.OrderBy(c => c.Gender).ThenBy(c => c.Name).ToList();
            return View(categories);
        }

        // GET: /AdminCategory/Create
        public ActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: /AdminCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: /AdminCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // POST: /AdminCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: /AdminCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // POST: /AdminCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var category = db.Categories.Find(id);
            bool inUse = db.Products.Any(p => p.CategoryID == id);

            if (category != null && !inUse)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            else
            {
                TempData["Error"] = "Cannot delete a category that still has products assigned to it.";
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
