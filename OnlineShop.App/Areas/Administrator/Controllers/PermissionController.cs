using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using Microsoft.Ajax.Utilities;
using OnlineShop.App.Securities;
using OnlineShop.Core.Services.Permissions;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class PermissionController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        public PermissionController(IUnitOfWork unitOfWork, IPermissionService permissionService)
            : base(unitOfWork)
        {
            _permissionService = permissionService;
        }
        //
        // GET: /Administrator/Permission/
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult GetPermissions(DataTablesParam request)
        {
            var permission = _permissionService.GetAll();
            return DataTablesResult.Create(permission,request);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new Role());
        }

        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(Role model)
        {
            _permissionService.Insert(model);
            return Json(new {result = true});
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }
            var entity = _permissionService.FindById(id.Value);
            return View(entity);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(Role model)
        {
            if (ModelState.IsValid)
            {
                _permissionService.Update(model);
                return Json(new {result = true});
            }
            return Json(new {result = false});
        }

        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                _permissionService.Delete(id.Value);
                return Json(new {result = true});
            }
            return Json(new {result = false});
        }
	}
}