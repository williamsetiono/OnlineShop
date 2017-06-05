using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using Newtonsoft.Json;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Resources;
using OnlineShop.Core.Services.Coupons;
using OnlineShop.Core.Services.Orders;
using OnlineShop.Core.Services.Products;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;
using OnlineShop.Models.Entities;
using PayPal.Api;

namespace OnlineShop.App.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IOrdersService _ordersService;
        private readonly ICouponService _couponService;
        private readonly IDictionary<string, string> _settingPaypal;
        private readonly string _currency;
        private Payment payment;
        public OrdersController(ICouponService couponService,IProductService productService, IOrdersService ordersService, IUserService userService,ISettingService settingService)
            : base(userService, settingService)
        {
            _productService = productService;
            _ordersService = ordersService;
            _settingPaypal = GetSettingPaypal();
            _currency = GetSettingCurrency();
            _couponService = couponService;
        }
        [MyAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize]
        public ActionResult GetAll(DataTablesParam request)
        {
            var user = GetCurrentUser();
            var solve = Constants.StatusDictionary[(int) Constants.EnumOrderStatus.Solve];
            var pendding = Constants.StatusDictionary[(int) Constants.EnumOrderStatus.Pending];
            if (user == null) return null;
            var entities = _ordersService.GetAll().Where(a => a.AccountId == user.Id).Select(a => new OrdersUserViewModel
            {
                Id = a.Id,
                StrStatus = a.Status == 1 ? solve : pendding,
                Total = a.OrderDetails.Sum(x => x.Quantity * x.Price),
                Amount = a.OrderDetails.Sum(x => x.Quantity),
                CreatedDate = a.CreatedDate
            });
            return DataTablesResult.Create(entities, request);
        }
        [AllowAnonymous]
        public ActionResult ViewCart()
        {
            try
            {
                var model = Session[Constants.CookieCart] = GetCartItems();
                return View(model ?? new CartViewModel());
            }
            catch (Exception)
            {

                return View(new CartViewModel());
            }
      
        }
        [MyAuthorize]
        public ActionResult Buy()
        {
            var message = Resource.MsgBuyItemFaild;
                try
                {
                    var model = GetSessionCart();
                    model.UserName = User.Identity.Name;
                    var id =_ordersService.Insert(model);
                    //RemoveCart();
                    message = Resource.MsgBuyItemSuccess;
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = true,
                        Message = message
                    };
                    if (Request.Cookies[Constants.CookieCart] != null)
                    {
                        var c = new HttpCookie(Constants.CookieCart);
                        c.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(c);
                    }
                    return RedirectToAction("OrderDetail", "Orders", new { id = id });
                }

                catch (Exception ex)
                {
                    message = ex.Message;
                }
            TempData[Constants.NotifyMessage] = new NotifyModel()
            {
                Result = false,
                Message = message
            };
            return RedirectToAction("ViewCart");
        }

        [MyAuthorize]
        public ActionResult OrderDetail(int? id)
        {
            if (!id.HasValue) { return RedirectToAction("Index", "Orders"); }
            var entity = _ordersService.FindById(id.Value);
            if (entity == null) { return RedirectToAction("Index", "Orders"); }
            var model = new OrderDetailViewModel()
            {
                Status = entity.Status,
                Id = entity.Id,
                Couponid = entity.CouponId != null ? entity.Coupon.Id : null,
                Discount =  entity.Coupon != null ? entity.Coupon.Discount : 0,
                Items = entity.OrderDetails.Select(a => new CartItemViewModel()
                {
                    Price = a.Price,
                    Quantity = a.Quantity,
                    ProductName = a.Product != null ? a.Product.Name : "",
                    Description = a.Product != null ? a.Product.Description : "",
                    Image = a.Product != null && a.Product.Images != null ? a.Product.Images.FirstOrDefault().Name : "",
                }).ToList()
            };
            return View(model);
        }

        [MyAuthorize]
        public ActionResult Confirm()
        {
            try
            {
                var apiContext = GetAPIContext();
                var guid = Request.Params["guid"];
                string payerId = Request.Params["PayerID"];
                var executedPayment = ExecutePayment(apiContext, payerId, Session[guid].ToString());

                if (executedPayment.state.ToLower() != "approved")
                {
                    return View("FailureView");
                }
                var cart = GetSessionCart();
                if (cart.Token.Equals(guid))
                {
                    _ordersService.Insert(cart);
                    Session[Constants.CookieCart] = new CartViewModel();
                }
                return RedirectToAction("Index", "Orders");
            }
            catch (Exception ex)
            {
                
                throw;
            }
           
            return null;
        }

        [MyAuthorize]
        public ActionResult CheckOut()
        {
            try
            {
                var cart = GetSessionCart();
                cart.UserName = User.Identity.Name;
                Session[Constants.CookieCart] = cart;
                return View(cart);
            }
            catch (Exception ex)
            {
                return View(new CartViewModel());
            }
        }

        [MyAuthorize]
        [HttpPost]
        public ActionResult CheckOut(string Coupon)
        {
            var apiContext = GetAPIContext();
            var payerId = Request.Params["PayerID"];
            var cart = GetSessionCart();
            var now = DateTime.Now;
            Coupon coupon = null;
            if (!string.IsNullOrWhiteSpace(Coupon))
            {
                coupon = _couponService.FindById(Coupon);
                if (coupon != null && !coupon.Status && (now < coupon.Start_Date || now > coupon.End_Date))
                {
                    coupon = null;
                }
            }
            if (cart == null)
            {
                TempData[Constants.NotifyMessage] = new NotifyModel() {Message = Resource.EmptyCart, Result = false};
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var guid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                       "/Orders/Confirm?";

                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution

                    //var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    cart.Token = guid;
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, cart,coupon);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            Session.Add(guid, createdPayment.id);
                            return Redirect(lnk.href);
                        }
                    }

                    // saving the paymentID in the key guid
                }



            }
            catch (Exception ex)
            {
            }
            return null;
        }
        //public ActionResult CheckOut(int? id)
        //{
        //    var apiContext = GetAPIContext();
        //    string payerId = Request.Params["PayerID"];
        //    if (!id.HasValue) { return RedirectToAction("Index", "Orders"); }
        //    var entity = _ordersService.FindById(id.Value);
        //    if (entity == null || entity.Status != (int)Constants.EnumOrderStatus.Pending) { return RedirectToAction("Index", "Orders"); }
        //    try
        //    {
        //        var guid = Guid.NewGuid().ToString();
        //        if (string.IsNullOrEmpty(payerId))
        //        {
        //            string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
        //               "/Orders/Confirm?";

        //            //guid we are generating for storing the paymentID received in session
        //            //after calling the create function and it is used in the payment execution

        //            //var guid = Convert.ToString((new Random()).Next(100000));

        //            //CreatePayment function gives us the payment approval url
        //            //on which payer is redirected for paypal account payment

        //            var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, entity);

        //            //get links returned from paypal in response to Create function call

        //            var links = createdPayment.links.GetEnumerator();

        //            string paypalRedirectUrl = null;

        //            while (links.MoveNext())
        //            {
        //                Links lnk = links.Current;

        //                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
        //                {
        //                    //saving the payapalredirect URL to which user will be redirected for payment
        //                    Session.Add(guid, createdPayment.id);
        //                   return Redirect(lnk.href);
        //                }
        //            }

        //            // saving the paymentID in the key guid
        //        }
                

                  
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return null;
        //}
        #region Helpers
        private CartViewModel GetCartItems()
        {
            var cartItems = GetSessionCart();
            var cartResult = new CartViewModel();
            if (cartItems != null)
            {
                const string imgStore = Constants.ImagePath;
                cartItems.UserName = User.Identity.Name;
                foreach (var item in cartItems.Items)
                {
                    var product = _productService.FindById(item.ProductId);
                    if (product == null) continue;
                    var img = product.Images != null && product.Images.Any()
                        ? product.Images.Any(a => a.Main == true)
                            ? imgStore + "/" + product.Images.FirstOrDefault(a => a.Main == true).Name
                            : imgStore + "/" + product.Images.FirstOrDefault().Name
                        : null;
                    cartResult.Items.Add(new CartItemViewModel()
                    {
                        Quantity = item.Quantity,
                        ProductId = product.Id,
                        Price = product.Price,
                        ProductName = product.Name,
                        Image = img,
                        Description = product.Description
                    });
                }
            }
            return cartResult;
        }

        private void RemoveCart()
        {
            Request.Cookies.Remove(Constants.CookieCart);
            if (Request.Cookies[Constants.CookieCart] != null)
            {
                var c = new HttpCookie(Constants.CookieCart);
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, CartViewModel entity,Coupon coupon)
        {
            //similar to credit card create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            var transactionList = new List<Transaction>();
            var items = entity.Items;
            var discount = (decimal)0.0;
            if (coupon != null)
            {
                entity.CouponId = coupon.Id;
                discount = coupon.Discount;
            }
            Session[Constants.CookieCart] = entity;
            var value = JsonConvert.SerializeObject(entity);
            value = EncryptString(value);
            var cookie = new HttpCookie(Constants.CookieCart,value);
            Response.Cookies.Add(cookie);
            foreach (var item in items)
            {
                itemList.items.Add( new Item()
                {
                    currency = _currency,
                    description = item.Description,
                    name = item.ProductName,
                    price = item.Price.ToString(),
                    quantity = item.Quantity.ToString(),
                    sku = "sku",
                });
            }
            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                shipping_discount = coupon == null ? "0" : discount.ToString("##.##"),
                subtotal = ((decimal)items.Sum(item => item.Price * item.Quantity)).ToString("##.##")
            };
            var amount = new Amount()
            {
                currency = _currency,
                total = ((decimal)items.Sum(item => item.Price * item.Quantity).Value - discount).ToString("##.##"),
                details = details
            };
            transactionList.Add(new Transaction()
            {
                description = Guid.NewGuid().ToString() + "_" + entity.Token,
                invoice_number = DateTime.Now.ToBinary() + "_" + entity.Token,
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "Sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }
        public APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
        private string GetAccessToken()
        {
            // getting accesstocken from paypal                
            string accessToken = new OAuthTokenCredential
        (_settingPaypal["ClientId"], _settingPaypal["ClientSecretKey"], GetConfig()).GetAccessToken();

            return accessToken;
        }
        public  Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>()
            {
                {"mode",_settingPaypal["ClientMode"]},
                {"connectionTimeout",_settingPaypal["ClientConnectionTimeOut"]},
                {"requestRetries",_settingPaypal["ClientRequestRetries"]},
                {"clientId",_settingPaypal["ClientId"]},
                {"clientSecret",_settingPaypal["ClientSecretKey"]}
            };
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        public  string EncryptString(string plainText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(Constants.VIKey);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Constants.SaltKey, null);
            byte[] keyBytes = password.GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        //Decrypt
        public  string DecryptString(string cipherText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(Constants.VIKey);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Constants.SaltKey, null);
            byte[] keyBytes = password.GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
        #endregion
    }
}