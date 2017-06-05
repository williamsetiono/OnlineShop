using System;
using System.Linq;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using OnlineShop.App.Controllers;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.Coupons;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Services.Coupons;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class CouponController : BaseController
    {
        //
        // GET: /Coupon/
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService,IUserService userService, ISettingService settingService)
            : base(userService,settingService)
        {
            _couponService = couponService;
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult GetAll(DataTablesParam request)
        {
           
            var model = _couponService.GetAll().Select(a=> new CouponViewModel()
            {
                Id = a.Id,
                StrStatus = a.Status ? "Active" : "Inactive",
                Discount = a.Discount,
                EndDate = a.End_Date,
                StartDate = a.Start_Date
            });
            return DataTablesResult.Create(model, request);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new CouponViewModel{StartDate = DateTime.Now,EndDate = DateTime.Now});
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(CouponViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _couponService.Insert(model);
                    TempData[Constants.NotifyMessage] = new NotifyModel { Result = true, Message = string.Format("Create {0} success", "coupon") };
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel { Result = false, Message = string.Format("Create {0} failed, error: {1}","coupon",ex.Message) };
                }

            }
            else
            {
                TempData[Constants.NotifyMessage] = new NotifyModel {Result = false, Message = "Invalid data"};
            }
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData[Constants.NotifyMessage] = new NotifyModel { Result = false, Message = string.Format("{0} is not found","Coupon") };
                return RedirectToAction("Index");
            }
            var entity = _couponService.FindById(id);
            var model = new CouponViewModel()
            {
                Id = entity.Id,
                Status = entity.Status,
                Discount = entity.Discount,
                EndDate = entity.End_Date,
                StartDate = entity.Start_Date
            };
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(CouponViewModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    _couponService.Update(model);

                    TempData[Constants.NotifyMessage] = new NotifyModel
                    {
                        Result = true,
                        Message = string.Format("Update {0} success", "coupon")
                    };
                    return Redirect("Index");
                }
                catch (Exception ex)
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel
                    {
                        Result = true,
                        Message = string.Format("Update {0} failed, error: {1}", "coupon", ex.Message)
                    };

                }
            }
            else
            {
                TempData[Constants.NotifyMessage] = new NotifyModel { Result = true, Message = string.Format("Update {0} failed, error: {1}", "coupon", "Invalid data") };
            }
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                _couponService.Delete(id);
                return Json(new {result = true});
            }
            catch (Exception)
            {
            }
            return Json(new {result = false});
        }
	}
}