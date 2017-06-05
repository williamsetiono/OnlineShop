using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;
using OnlineShop.Models.Entities;

namespace OnlineShop.App.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUserService _userService;
        private readonly ISettingService _settingService;
        private Dictionary<string, string> _Setting;

        public BaseController(IUserService userService, ISettingService settingService)
        {
            _userService = userService;
            _settingService = settingService;
            ViewBag.BaseSetting = GetBaseSetting();

        }

        protected bool IsCurrentuser()
        {
            if (!User.Identity.IsAuthenticated) return false;
            var entity = _userService.FindByName(User.Identity.Name);
            return entity != null;
        }
        protected Account GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated) return null;
            var entity = _userService.FindByName(User.Identity.Name);
            return entity;
        }

        protected IDictionary<string, string> GetSettingPaypal()
        {
            return _settingService.GetAll().Where(a => a.Id.StartsWith("Client")).ToDictionary(a=>a.Id,a=>a.Value);
        }
        protected string GetSettingCurrency()
        {
            return _settingService.GetAll().Any(a => a.Id.Equals("Currency")) ?  _settingService.GetAll().First(a => a.Id.Equals("Currency")).Value : "USD";
        }

        protected Dictionary<string, string> GetBaseSetting()
        {
            return _settingService.GetAll()
                .Where(a => a.Type.Equals("BaseSetting"))
                .ToDictionary(a => a.Id, a => a.Value);
        }
        protected CartViewModel GetSessionCart()
        {
            var cartObj = Session[Constants.CookieCart];
            CartViewModel cart = null;
            try
            {
                if (cartObj != null) cart = (CartViewModel)cartObj;
            }
            catch (Exception)
            {
                return new CartViewModel();
            }
            return cart ?? new CartViewModel();
        }

	}
}