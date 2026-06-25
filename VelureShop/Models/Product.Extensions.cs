using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelureShop.Models
{
    public partial class Product
    {
        [NotMapped]
        public string DisplayImage
        {
            get
            {
                return string.IsNullOrEmpty(ImagePath)
                    ? null
                    : "/Images/products/" + ImagePath;
            }
        }
    }
}