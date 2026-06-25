using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models;

namespace VelureShop.Controllers
{
    public class HomeController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        public ActionResult Index()
        {
            var womenBestsellers = db.Products
                .Where(p => p.IsActive && p.Category.Gender == "Women")
                .OrderByDescending(p => p.ProductID)
                .Take(4)
                .ToList();

            var menBestsellers = db.Products
                .Where(p => p.IsActive && p.Category.Gender == "Men")
                .OrderByDescending(p => p.ProductID)
                .Take(4)
                .ToList();

            ViewBag.WomenBestsellers = womenBestsellers;
            ViewBag.MenBestsellers = menBestsellers;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
