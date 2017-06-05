using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace OnlineShop.Core.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        [Range(0,Int32.MaxValue)]
        public int? Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double? Price { get; set; }
        public int Status { get; set; }
        [Required]
        public string Description { get; set; }
        public string Category { get; set; }
        [DisplayName("Category")]
        [Required]
        public int? CategoryId { get; set; }
        public int MainImg { get; set; }
        public string[] ImageList { get; set; }
        public ICollection<HttpPostedFileBase> Images { get; set; }
        public string Image { get; set; }
        public Dictionary<int,string> Categories { get; set; }
        public Dictionary<int, string> StatusDictionary { get; set; }
    }
}
