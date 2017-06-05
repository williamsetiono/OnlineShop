using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.Categories;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Services.Categories;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class CategoryController : BaseAdminController
    {
        //
        // GET: /Administrator/Category/
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService, IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _categoryService = categoryService;
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult GetAll(DataTablesParam dataTablesParam)
        {
            var model = _categoryService.GetAll().Select(a => new CategoryViewModel()
            {
                CategoryName = a.Name,
                Ancestor = a.Ancestor,
                Id = a.Id,
                ParentCategory = a.Category2 == null ? "" : a.Category2.Name
            });
            return DataTablesResult.Create(model, dataTablesParam,null,a=>a.Ancestor);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");
            var entity = _categoryService.FindById(id.Value);
            if (entity == null) return RedirectToAction("Index");
            var model = new CategoryViewModel()
            {
                DictionaryCategory = GetCategoryDictionary(id),
                CategoryName = entity.Name,
                ParentId = entity.ParentId,
                Id = entity.Id
            };
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = new Category()
                    {
                        Name = model.CategoryName,
                        ParentId = model.ParentId == -1 ? null : model.ParentId,
                        Id = model.Id
                    };
                    _categoryService.Update(entity);
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = true,
                        Message = string.Format("Update {0} success", "category " + model.CategoryName)
                    };
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = false,
                        Message = string.Format("Update {0} failed", "Error: " + ex.Message)
                    };
                }

            }
            else
            {
                TempData[Constants.NotifyMessage] = new NotifyModel()
                {
                    Result = false,
                    Message = string.Format("Update {0} failed", "Error: " + "Invalid data")
                };
            }
            model.DictionaryCategory = GetCategoryDictionary(null);
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new CategoryViewModel() { DictionaryCategory = GetCategoryDictionary(null) });
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = new Category()
                    {
                        Name = model.CategoryName,
                        ParentId = model.ParentId == -1 ? null : model.ParentId
                    };
                    _categoryService.Insert(entity);
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = true,
                        Message = string.Format("Insert {0} success", "category " + model.CategoryName)
                    };
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //return Json(new { result = true, message = ex.Message });
                    model.DictionaryCategory = GetCategoryDictionary(null);
                    TempData[Constants.NotifyMessage] = new NotifyModel()
                    {
                        Result = false,
                        Message = string.Format("Update failed, {0}", "Error: " + ex.Message)
                    };
                    return View(model);
                }

            }
            else
            {
                TempData[Constants.NotifyMessage] = new NotifyModel()
                {
                    Result = false,
                    Message = string.Format("Update failed, {0}", "Error: " + "Invalid data")
                };
            }
            model.DictionaryCategory = GetCategoryDictionary(null);
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return Json(new NotifyModel(){ Result = false, Message = "Category is not found" });
            }

            try
            {
                _categoryService.Delete(id.Value);
                return Json(new NotifyModel { Result = true, Message = string.Format("Delete {0} success", "category") });
            }
            catch (Exception ex)
            {
                return Json(new NotifyModel { Result = false, Message = ex.Message });
            }
         
        }
	}
}