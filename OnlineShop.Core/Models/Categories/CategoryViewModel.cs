using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.Categories
{
    public class CategoryViewModel
    {

        public string Ancestor { get; set; }
        public int Level { get; set; }
        public int Id { get; set; }
        [Required]
        [Display(Name = @"Category name")]
        public string CategoryName { get; set; }
        [Display(Name = @"Subcategory of")]
        public int? ParentId { get; set; }
        public string ParentCategory { get; set; }
        public Dictionary<int,string> DictionaryCategory { get; set; }
    }
}
