using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelureShop.Models
{
    public partial class OrderItem
    {
        [NotMapped]
        public decimal LineTotal
        {
            get { return UnitPrice * Quantity; }
        }
    }
}