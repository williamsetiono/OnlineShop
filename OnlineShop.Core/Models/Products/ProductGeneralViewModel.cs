using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.Products
{
    public class ProductGeneralViewModel
    {
        public int? Id { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public string ProductName { get; set; }
    }
}
