using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

using VelureShop.Models;

namespace VelureShop.Controllers
{
    public class AdminOrderController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        private bool IsAdmin()
        {
            return Session["UserID"] != null && Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // GET: /AdminOrder
        public ActionResult Index(string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var query = db.Orders.Include(o => o.User).Include(o => o.Address);

            var orders = string.IsNullOrEmpty(status)
                ? query.OrderByDescending(o => o.CreatedAt).ToList()
                : query.Where(o => o.Status == status).OrderByDescending(o => o.CreatedAt).ToList();

            ViewBag.StatusFilter = status;
            return View(orders);
        }

        // GET: /AdminOrder/Details/5
        public ActionResult Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders
                .Include("User")
                .Include("Address")
                .Include("Payments")
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null) return HttpNotFound();

            ViewBag.Items = db.OrderItems
                .Include("Product")
                .Where(oi => oi.OrderID == id)
                .ToList();

            // Pass payment separately so the view doesn't need LINQ
            ViewBag.Payment = db.Payments.FirstOrDefault(p => p.OrderID == id);

            return View(order);
        }

        // POST: /AdminOrder/UpdateStatus
        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;

                if (status == "Delivered")
                {
                    var payment = db.Payments.FirstOrDefault(p => p.OrderID == orderId);
                    if (payment != null && payment.Status == "Pending")
                    {
                        payment.Status = "Paid";
                        payment.PaidAt = System.DateTime.Now;
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = orderId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
