using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    namespace VelureShop.Models.ViewModels
    {
        public class ShopListViewModel
        {
            public List<Product> Products { get; set; }
            public List<Category> Categories { get; set; }
            public string Gender { get; set; }            // "Women" or "Men"
            public int? SelectedCategoryID { get; set; }
            public string SortBy { get; set; }             // "newest", "price_asc", "price_desc"
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int TotalCount { get; set; }
        }

        public class CartViewModel
        {
            public List<CartItem> Items { get; set; }
            public decimal Subtotal { get; set; }
            public decimal Shipping { get; set; }
            public decimal Tax { get; set; }
            public decimal Total { get; set; }

            public CartViewModel()
            {
                Items = new List<CartItem>();
            }
        }

        public class CheckoutViewModel
        {
            public List<CartItem> Items { get; set; }
            public decimal Subtotal { get; set; }
            public decimal Shipping { get; set; }
            public decimal Tax { get; set; }
            public decimal Total { get; set; }

            // Contact
            public string Email { get; set; }
            public string Phone { get; set; }

            // Shipping address
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Street { get; set; }
            public string Area { get; set; }
            public string City { get; set; }
            public string Province { get; set; }

            public CheckoutViewModel()
            {
                Items = new List<CartItem>();
            }
        }

        public class AdminDashboardViewModel
        {
            public int TotalProducts { get; set; }
            public int TotalOrders { get; set; }
            public int PendingOrders { get; set; }
            public int TotalCustomers { get; set; }
            public decimal TotalRevenue { get; set; }
            public List<Order> RecentOrders { get; set; }

            public AdminDashboardViewModel()
            {
                RecentOrders = new List<Order>();
            }
        }
    }

