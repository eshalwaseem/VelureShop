using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VelureShop.Models;

namespace VelureShop.Controllers
{
    public class AccountController : Controller
    {
        private VelureShopEntities db = new VelureShopEntities();

        // =========================
        // SIGNUP - GET
        // =========================
        public ActionResult Signup()
        {
            return View();
        }

        // =========================
        // SIGNUP - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(string fullName, string email, string password, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please fill in all required fields.";
                return View();
            }

            var existingUser = db.Users.FirstOrDefault(x => x.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "An account with this email already exists.";
                return View();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password, // NOTE: plain text to match the supplied schema; hash in production
                Phone = phone,
                Role = "Customer",
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Create an empty cart for the new customer
            db.Carts.Add(new Cart { UserID = user.UserID, UpdatedAt = DateTime.Now });
            db.SaveChanges();

            // Auto sign-in
            Session["UserID"] = user.UserID;
            Session["Name"] = user.FullName;
            Session["Role"] = user.Role;

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // LOGIN - GET
        // =========================
        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // =========================
        // LOGIN - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string returnUrl = null)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["Name"] = user.FullName;
                Session["Role"] = user.Role;

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "AdminDashboard");
                }

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // =========================
        // LOGOUT
        // =========================
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
	}
}