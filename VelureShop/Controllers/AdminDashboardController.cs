using System;
using System.Linq;
using System.Web.Mvc;
using VelureShop.Models;
using VelureShop.Models.ViewModels;

namespace VelureShop.Controllers
{
    public class AdminDashboardController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        private bool IsAdmin()
        {
            return Session["UserID"] != null && Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // GET: /AdminDashboard
        public ActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "AdminDashboard") });

            var vm = new AdminDashboardViewModel
            {
                TotalProducts = db.Products.Count(p => p.IsActive),
                TotalOrders = db.Orders.Count(),
                PendingOrders = db.Orders.Count(o => o.Status == "Pending"),
                TotalCustomers = db.Users.Count(u => u.Role == "Customer"),
                TotalRevenue = db.Orders.Where(o => o.Status != "Cancelled").Sum(o => (decimal?)o.Total) ?? 0,
                RecentOrders = db.Orders.OrderByDescending(o => o.CreatedAt).Take(5).ToList()
            };

            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
