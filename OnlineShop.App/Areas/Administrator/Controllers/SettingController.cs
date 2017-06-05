using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.Settings;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class SettingController : BaseAdminController
    {
        //
        // GET: /Administrator/Setting/
        private readonly ISettingService _settingService;
        public SettingController(IUnitOfWork unitOfWork, ISettingService settingService) : base(unitOfWork)
        {
            _settingService = settingService;
        }


        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult GetAll(DataTablesParam request)
        {
            var models = _settingService.GetAll();
            return DataTablesResult.Create(models, request);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            
            return View();
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(SettingVIewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _settingService.Insert(model);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                }
            }
            return View(model);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            var entity = _settingService.FindById(id);
            if (entity == null) return RedirectToAction("Index");
            var model = new SettingVIewModel()
            {
                Id = entity.Id,
                Value = entity.Value,
                Type = entity.Type
            };
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(SettingVIewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _settingService.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                }
               
            }
            return View(model);
        }

        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return Json(new {result = false});
            try
            {
                _settingService.Delete(id.Value);
                return Json(new { result = true });
            }
            catch (Exception)
            {
            }
            return Json(new { result = false });
        }

        
	}
}