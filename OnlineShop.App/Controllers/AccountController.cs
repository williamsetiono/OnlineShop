using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Users;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Resources;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;
using OnlineShop.Core.Utils;
using OnlineShop.Models.Entities;

namespace OnlineShop.App.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUserService userService, ISettingService settingService)
            : base(userService, settingService)
        {
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(new UserClientLoginViewModel()
            {
                UserLoginViewModel = new UserLoginViewModel(),
                RegisterViewModel = new RegisterViewModel()
            });
        }
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
        {
            return PartialView("_LoginPartial");
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(UserLoginViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var pwd = UtilEncrypt.GetMd5Hash(model.Password);
                var acc = _userService.GetAll().FirstOrDefault(a => a.Username.Equals(model.UserName) && a.Password.Equals(pwd));
                if (acc != null)
                {
                    StoreCookie(acc);
                    if (string.IsNullOrWhiteSpace(returnUrl)) return RedirectToAction("Index", "Home");
                    return Redirect(returnUrl);
                }
                else
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = false,
                        Message = string.Format(Resource.DataIsNotFound, model.UserName)
                    };
                    return RedirectToAction("Login", "Account");
                }
            }
            TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = string.Format(Resource.InvalidData, Resource.User) };
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return PartialView("_RegisterPartial");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (!model.Password.Equals(model.ConfirmPassword))
                    {
                        ModelState["Password"].Errors.Add("Password is not match");
                        return RedirectToAction("Login", "Account");
                    }
                    _userService.Insert(model);
                    var pwd = UtilEncrypt.GetMd5Hash(model.Password);

                    var acc = _userService.GetAll().FirstOrDefault(a => a.Username.Equals(model.UserName) && a.Password.Equals(pwd));
                    if (acc != null)
                    {
                        StoreCookie(acc);
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    var notify = new NotifyModel()
                    {
                        Result = false,
                        Message = Resource.InternalException
                    };
                    if (ex.InnerException == null)
                    {
                        notify.Message = ex.Message;
                    }
                    TempData[Constants.NotifyMessage] = notify;
                    return RedirectToAction("Login", "Account");
                }
            }
            TempData[Constants.NotifyMessage] = new NotifyModel()
            {
                Result = true,
                Message = string.Format(Resource.InvalidData, Resource.User)
            };
            return RedirectToAction("Login", "Account");
            // If we got this far, something failed, redisplay form
        }

        private void StoreCookie(Account acc)
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
        }
    }

}
