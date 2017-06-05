using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using datatables.Utils.DatatableModels;
using Newtonsoft.Json;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Core.Models.Users;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Resources;
using OnlineShop.Core.Services.Orders;
using OnlineShop.Core.Utils;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class HomeController : BaseAdminController
    {
        //
        // GET: /Administrator/Home/
        private readonly IOrdersService _ordersService;
        public HomeController(IOrdersService ordersService, IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _ordersService = ordersService;
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            //TempData[Constants.NotifyMessage] = new NotifyModel() {Result = false,Message = "test"};
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login");
            ViewBag.currencySign = GetCurrencySign();
            return View();
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult GetAll(DataTablesParam request)
        {
            var now = DateTime.Now;
            var weekAgo = now.AddDays(-7);
            var orders = _ordersService.GetAll().Where(a => DbFunctions.TruncateTime(a.CreatedDate) >= DbFunctions.TruncateTime(weekAgo)).Select(a => new OrdersViewModel
            {
                Id = a.Id,
                StrStatus = a.Status == 1 ? "Active" : "Inactive",
                
                CreatedDate = a.CreatedDate,
                User = a.Account.Username,
                Total = a.OrderDetails.Sum(x => x.Price * x.Quantity)
            });
            return DataTablesResult.Create(orders, request);
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var pwd = UtilEncrypt.GetMd5Hash(model.Password);
                var acc = UnitOfWork.Repository<Account>().GetAll(a => a.Username.Equals(model.UserName) && a.Password.Equals(pwd)).FirstOrDefault();
                if (acc != null)
                {
                    var ma = new MyAccount();
                    ma.Username = acc.Username;
                    ma.Fullname = acc.FullName;
                    ma.Roles = acc.Role.Name;
                    var fat = new FormsAuthenticationTicket(1, "octopusAuth", DateTime.Now, DateTime.Now.AddMinutes(15), false, JsonConvert.SerializeObject(ma));
                    var ck = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(fat))
                    {
                        Expires = DateTime.Now.AddMinutes(15)
                    };
                    Response.Cookies.Add(ck);
                    if (string.IsNullOrWhiteSpace(returnUrl)) return RedirectToAction("Index", "Home",new RouteValueDictionary(Constants.AreaAdmin));
                    return Redirect(returnUrl);
                }
                else
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = false,
                        Message = string.Format(Resource.DataIsNotFound, model.UserName)
                    };
                    return RedirectToAction("Login", "Home", new RouteValueDictionary(Constants.AreaAdmin));
                }
            }
            TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = string.Format(Resource.InvalidData, Resource.User) };
            return RedirectToAction("Login", "Home", new RouteValueDictionary(Constants.AreaAdmin));
        }
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home", Constants.AreaAdmin);
        }
	}
}