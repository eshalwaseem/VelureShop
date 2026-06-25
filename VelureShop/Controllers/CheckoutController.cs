using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models.ViewModels;
using VelureShop.Models;

namespace VelureShop.Controllers
{
    public class CheckoutController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        private const decimal FreeShippingThreshold = 50.00m;
        private const decimal ShippingFlatRate = 250.00m;
        private const decimal TaxRate = 0.05m;

        private bool IsLoggedIn()
        {
            return Session["UserID"] != null;
        }

        // GET: /Checkout
        public ActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });

            int userId = (int)Session["UserID"];
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId);
            var items = cart == null
                ? new System.Collections.Generic.List<CartItem>()
                : db.CartItems.Where(ci => ci.CartID == cart.CartID).ToList();

            if (items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var user = db.Users.Find(userId);
            var defaultAddress = db.Addresses.FirstOrDefault(a => a.UserID == userId && a.IsDefault)
                                  ?? db.Addresses.FirstOrDefault(a => a.UserID == userId);

            decimal subtotal = items.Sum(i => i.Product.Price * i.Quantity);
            decimal shipping = subtotal >= FreeShippingThreshold || subtotal == 0 ? 0 : ShippingFlatRate;
            decimal tax = Math.Round(subtotal * TaxRate, 2);

            var vm = new CheckoutViewModel
            {
                Items = items,
                Subtotal = subtotal,
                Shipping = shipping,
                Tax = tax,
                Total = subtotal + shipping + tax,
                Email = user.Email,
                Phone = user.Phone
            };

            if (defaultAddress != null)
            {
                var nameParts = (defaultAddress.FullName ?? "").Split(new[] { ' ' }, 2);
                vm.FirstName = nameParts.Length > 0 ? nameParts[0] : "";
                vm.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                vm.Street = defaultAddress.Street;
                vm.Area = defaultAddress.Area;
                vm.City = defaultAddress.City;
                vm.Province = defaultAddress.Province;
            }

            return View(vm);
        }

        // POST: /Checkout/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(CheckoutViewModel form)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserID"];
            var cart = db.Carts.FirstOrDefault(c => c.UserID == userId);
            var items = cart == null
                ? new System.Collections.Generic.List<CartItem>()
                : db.CartItems.Where(ci => ci.CartID == cart.CartID).ToList();

            if (items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (string.IsNullOrWhiteSpace(form.FirstName) || string.IsNullOrWhiteSpace(form.Street)
                || string.IsNullOrWhiteSpace(form.City) || string.IsNullOrWhiteSpace(form.Province))
            {
                ViewBag.Error = "Please fill in all required shipping fields.";
                form.Items = items;
                RecalculateTotals(form, items);
                return View("Index", form);
            }

            // Save / reuse shipping address
            var address = new Address
            {
                UserID = userId,
                FullName = (form.FirstName + " " + form.LastName).Trim(),
                Street = form.Street,
                Area = form.Area,
                City = form.City,
                Province = form.Province,
                IsDefault = !db.Addresses.Any(a => a.UserID == userId)
            };
            db.Addresses.Add(address);
            db.SaveChanges();

            decimal subtotal = items.Sum(i => i.Product.Price * i.Quantity);
            decimal shipping = subtotal >= FreeShippingThreshold || subtotal == 0 ? 0 : ShippingFlatRate;
            decimal tax = Math.Round(subtotal * TaxRate, 2);

            var order = new Order
            {
                UserID = userId,
                AddressID = address.AddressID,
                Subtotal = subtotal,
                Shipping = shipping,
                Tax = tax,
                Total = subtotal + shipping + tax,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            db.Orders.Add(order);
            db.SaveChanges();

            foreach (var item in items)
            {
                db.OrderItems.Add(new OrderItem
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });

                // reduce stock
                var product = db.Products.Find(item.ProductID);
                if (product != null)
                {
                    product.StockQty = Math.Max(0, product.StockQty - item.Quantity);
                }
            }

            db.Payments.Add(new Payment
            {
                OrderID = order.OrderID,
                Method = "Cash on Delivery",
                Status = "Pending",
                PaidAt = null
            });

            // empty the cart
            db.CartItems.RemoveRange(items);

            db.SaveChanges();

            return RedirectToAction("Confirmation", new { id = order.OrderID });
        }

        // GET: /Checkout/Confirmation/5
        public ActionResult Confirmation(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserID"];
            var order = db.Orders
                .Where(o => o.OrderID == id && o.UserID == userId)
                .FirstOrDefault();

            if (order == null) return HttpNotFound();

            ViewBag.Items = db.OrderItems.Where(oi => oi.OrderID == order.OrderID).ToList();
            return View(order);
        }

        private void RecalculateTotals(CheckoutViewModel form, System.Collections.Generic.List<CartItem> items)
        {
            decimal subtotal = items.Sum(i => i.Product.Price * i.Quantity);
            decimal shipping = subtotal >= FreeShippingThreshold || subtotal == 0 ? 0 : ShippingFlatRate;
            decimal tax = Math.Round(subtotal * TaxRate, 2);
            form.Subtotal = subtotal;
            form.Shipping = shipping;
            form.Tax = tax;
            form.Total = subtotal + shipping + tax;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}