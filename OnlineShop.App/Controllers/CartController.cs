using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Resources;
using OnlineShop.Core.Services.Coupons;
using OnlineShop.Core.Services.Products;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;
using PayPal.Api;

namespace OnlineShop.App.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        public CartController(ICouponService couponService, IProductService productService, IUserService userService, ISettingService settingService)
            : base(userService, settingService)
        {
            _productService = productService;
            _couponService = couponService;
        }
        // GET: Cart
        [HttpPost]
        public ActionResult AddItem(int productId)
        {
            var cart = GetSessionCart();
            try
            {
                var product = _productService.FindById(productId);
                if (product == null || product.Status != (int)Constants.EnumProductStatus.Stocking)
                {
                    return
                        Json(new CartResultViewModel()
                        {
                            Result = false,
                            Message = string.Format(Resource.DataIsNotFound, Resource.Item),
                            Amount = cart.Items.Count
                        });
                }
                if (cart.Items.Any(a => a.ProductId == product.Id ))
                {
                    cart.Items.First(a => a.ProductId == product.Id).Quantity++;
                }
                else
                {
                    var image = product.Images.FirstOrDefault(a => a.Main.HasValue && a.Main.Value);
                    cart.Items.Add(new CartItemViewModel()
                    {
                        Description = product.Description,
                        Price = product.Price,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Image = image != null ? image.Name : Constants.DefaultImg
                    });
                }
                Session[Constants.CookieCart] = cart;
                return Json(new CartResultViewModel()
                {
                    Result = true,
                    Message = string.Format(Resource.AddedToCart),
                    Amount = cart.Items.Count
                });
            }
            catch (Exception)
            {
                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = string.Format(Resource.InternalException),
                    Amount = cart.Items.Count
                });
            }
        }

        [HttpPost]
        public ActionResult UpdateItem(int productId, int amount)
        {
            var cart = GetSessionCart();
            try
            {
                var product = _productService.FindById(productId);
                if (product == null || product.Status != (int)Constants.EnumProductStatus.Stocking)
                    return
                            Json(new CartResultViewModel()
                            {
                                Result = false,
                                Message = string.Format(Resource.DataIsNotFound, Resource.Item),
                                Amount = cart.Items.Count
                            });
                if (amount < 1)
                   return Json(new CartResultViewModel()
                    {
                        Result = false,
                        Message = string.Format(Resource.InvalidData, Resource.AmountItem),
                        Amount = cart.Items.Count
                    });
                if (cart.Items.Any(a => a.ProductId == product.Id))
                {
                    cart.Items.First(a => a.ProductId == product.Id).Quantity++;
                }
                Session[Constants.CookieCart] = cart;
                return Json(new CartResultViewModel()
                {
                    Result = true,
                    Message = Resource.HaveUpdatedCart,
                    Amount = cart.Items.Count
                });
            }
            catch (Exception ex)
            {

                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message =Resource.InternalException,
                    Amount = cart.Items.Count
                });
            }
          
        }

        [HttpPost]
        public ActionResult DeleteItem(int productId)
        {
            var cart = GetSessionCart();
            try
            {
                if (cart.Items.Any(a => a.ProductId == productId))
                {
                    var item = cart.Items.First(a => a.ProductId == productId);
                    cart.Items.Remove(item);
                    Session[Constants.CookieCart] = cart;
                    return Json(new CartResultViewModel()
                    {
                        Result = true,
                        Message = string.Format(Resource.DeleteItemSuccess, Resource.CartItem.ToLower()),
                        Amount = cart.Items.Count
                    });
                }
                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = string.Format(Resource.DataIsNotFound, Resource.CartItem.ToLower()),
                    Amount = cart.Items.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new CartResultViewModel()
                {
                    Result = false,
                    Message = Resource.InternalException,
                    Amount = cart.Items.Count
                });
            }
        }

        [HttpPost]
        public ActionResult CheckCoupon(string couponCode)
        {
            var cart = GetSessionCart();
            var now = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = _couponService.FindById(couponCode);
                    if (coupon == null || now < coupon.Start_Date || now >coupon.End_Date)
                    {
                        return Json(new CouponResultViewModel()
                        {
                            Result = false,
                            Message = string.Format(Resource.DataIsNotFound, Resource.HeaderCoupon),
                            Discount = 0,
                            Total = Convert.ToDecimal(cart.Total)
                        });
                    }
                    return Json(new CouponResultViewModel()
                    {
                        Result = true,
                        Message = string.Format(Resource.DataIsNotFound, Resource.HeaderCoupon),
                        Discount = coupon.Discount,
                        Total = Convert.ToDecimal(cart.Total) - coupon.Discount
                    });  
                }
                return Json(new CouponResultViewModel()
                {
                    Result = false,
                    Message = string.Format(Resource.InvalidData, Resource.HeaderCoupon),
                    Discount = 0,
                    Total = Convert.ToDecimal(cart.Total)
                });
            }
            catch (Exception ex)
            {
                return Json(new CouponResultViewModel()
                {
                    Result = true,
                    Message = Resource.InternalException,
                    Discount = 0,
                    Total = Convert.ToDecimal(cart.Total)
                });  
            }
        }
        #region Helper

        #endregion

    }
}