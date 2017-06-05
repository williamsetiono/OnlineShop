using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Resources;

namespace OnlineShop.Core.Models.Orders
{
    public class ShoppingCartViewModel
    {
        [Microsoft.Build.Framework.Required]
        public string User { get; set; }
        public ICollection<ShoppingCartItemViewModel> Items { get; set; }
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public ICollection<CartItemViewModel> Items { get; set; }
        [Display(Name = "CouponCode")]
        public string Couponid { get; set; }

        public decimal Discount { get; set; }
    }
    public class ShoppingCartItemViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

    public class CartViewModel
    {
        public CartViewModel()
        {
            Items = new Collection<CartItemViewModel>();
        }
        public string Token { get; set; }
        public string UserName { get; set; }
        public ICollection<CartItemViewModel> Items { get; set; }
        public double Total { get { return Items.Sum(x => x.Total); } }
        public string CouponId { get; set; }
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
        public double Total { get { return (!Price.HasValue ? 0 : Price.Value)*Quantity;}}
    }

    public class PayPalItems
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CartResultViewModel : NotifyModel
    {
        public int Amount { get; set; }
    }

    public class CouponResultViewModel : NotifyModel
    {
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
