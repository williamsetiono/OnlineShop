using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShop.Core.Models.Orders
{
    public class OrdersViewModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
        public Double Total { get; set; }
    }

    public class OrdersUserViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string StrStatus { get; set; }
        public int Amount { get; set; }
        public Double Total { get; set; }
    }
}
