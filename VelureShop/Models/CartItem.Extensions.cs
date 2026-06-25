using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelureShop.Models
{
    public partial class CartItem
    {
        [NotMapped]
        public decimal LineTotal
        {
            get { return Product != null ? Product.Price * Quantity : 0; }
        }
    }
}