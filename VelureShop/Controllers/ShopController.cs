using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models;
using VelureShop.Models.ViewModels;

namespace VelureShop.Controllers
{
    public class ShopController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();
        private const int PageSize = 6;

        // GET: /Shop/Women
        public ActionResult Women(int? categoryId, string sort, int page = 1)
        {
            return BuildListing("Women", categoryId, sort, page);
        }

        // GET: /Shop/Men
        public ActionResult Men(int? categoryId, string sort, int page = 1)
        {
            return BuildListing("Men", categoryId, sort, page);
        }

        private ActionResult BuildListing(string gender, int? categoryId, string sort, int page)
        {
            var query = db.Products
                .Where(p => p.IsActive && p.Category.Gender == gender);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId.Value);
            }

            switch (sort)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderByDescending(p => p.ProductID);
                    sort = "newest";
                    break;
            }

            int totalCount = query.Count();
            int totalPages = (int)System.Math.Ceiling(totalCount / (double)PageSize);
            if (page < 1) page = 1;

            var products = query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var categories = db.Categories
                .Where(c => c.Gender == gender)
                .ToList();

            var vm = new ShopListViewModel
            {
                Products = products,
                Categories = categories,
                Gender = gender,
                SelectedCategoryID = categoryId,
                SortBy = sort,
                CurrentPage = page,
                TotalPages = totalPages == 0 ? 1 : totalPages,
                TotalCount = totalCount
            };

            ViewBag.Title = "Shop " + gender;
            return View("Listing", vm);
        }

        // GET: /Shop/Details/5
        public ActionResult Details(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id && p.IsActive);
            if (product == null) return HttpNotFound();

            var related = db.Products
                .Where(p => p.IsActive && p.CategoryID == product.CategoryID && p.ProductID != product.ProductID)
                .Take(4)
                .ToList();

            ViewBag.Related = related;
            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
