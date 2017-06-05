using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Core.Models.Products
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string[] Imgs { get; set; }
        public string MainImg { get; set; }
        public string Status { get; set; }
    }
}
