using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models;
using VelureShop.Models.ViewModels;

namespace VelureShop.Controllers
{
    public class CartController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();
        private const decimal FreeShippingThreshold = 50.00m;
        private const decimal ShippingFlatRate = 250.00m;
        private const decimal TaxRate = 0.05m;

        private bool IsLoggedIn()
        {
            return Session["UserID"] != null;
        }

        private Cart GetOrCreateCart(int userId)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId);
            if (cart == null)
            {
                cart = new Cart { UserID = userId, UpdatedAt = DateTime.Now };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart;
        }

        // GET: /Cart
        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Cart") });

            int userId = (int)Session["UserID"];
            var cart = GetOrCreateCart(userId);

            var items = db.CartItems
                .Where(ci => ci.CartID == cart.CartID)
                .ToList();

            var vm = BuildCartViewModel(items);
            return View(vm);
        }

        private CartViewModel BuildCartViewModel(System.Collections.Generic.List<CartItem> items)
        {
            decimal subtotal = items.Sum(i => i.Product.Price * i.Quantity);
            decimal shipping = subtotal >= FreeShippingThreshold || subtotal == 0 ? 0 : ShippingFlatRate;
            decimal tax = System.Math.Round(subtotal * TaxRate, 2);

            return new CartViewModel
            {
                Items = items,
                Subtotal = subtotal,
                Shipping = shipping,
                Tax = tax,
                Total = subtotal + shipping + tax
            };
        }

        // POST: /Cart/Add
        [HttpPost]
        public ActionResult Add(int productId, int quantity = 1)
        {
            if (!IsLoggedIn())
            {
                return Json(new { success = false, redirect = Url.Action("Login", "Account") });
            }

            int userId = (int)Session["UserID"];
            var cart = GetOrCreateCart(userId);

            var existing = db.CartItems.FirstOrDefault(ci => ci.CartID == cart.CartID && ci.ProductID == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                db.CartItems.Add(new CartItem { CartID = cart.CartID, ProductID = productId, Quantity = quantity });
            }

            cart.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            int cartCount = db.CartItems.Where(ci => ci.CartID == cart.CartID).Sum(ci => ci.Quantity);

            return Json(new { success = true, cartCount = cartCount });
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public ActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var item = db.CartItems.FirstOrDefault(ci => ci.CartItemID == cartItemId);
            if (item != null)
            {
                if (quantity < 1) quantity = 1;
                item.Quantity = quantity;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public ActionResult Remove(int cartItemId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var item = db.CartItems.FirstOrDefault(ci => ci.CartItemID == cartItemId);
            if (item != null)
            {
                db.CartItems.Remove(item);
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
