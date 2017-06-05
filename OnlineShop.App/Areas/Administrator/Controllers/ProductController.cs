using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Products;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Services.Products;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class ProductController : BaseAdminController
    {
        //
        // GET: /Administrator/Product/
        private readonly IProductService _productService;
        public ProductController(IUnitOfWork unitOfWork,IProductService productService) : base(unitOfWork)
        {
            _productService = productService;
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.currencySign = GetCurrencySign();
            return View();
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult GetAll(DataTablesParam dataTablesParam)
        {
            var entities = _productService.GetAll();
            if (!string.IsNullOrEmpty(dataTablesParam.sSearch))
            {
                var searchParam = dataTablesParam.sSearch;
                var searchDate = new DateTime();
                int quantity = 0;double price = 0;
                double.TryParse(searchParam, out price);
                int.TryParse(searchParam, out quantity);
                DateTime.TryParse(searchParam, out searchDate);
                entities = entities.Where(a => 
                    a.Name.Contains(searchParam) || a.Description.Contains(searchParam) || 
                    (DateTime.MinValue != searchDate && DbFunctions.TruncateTime(a.CreatedDate) == searchDate ||
                    (!a.Quantity.HasValue || a.Quantity.Value == quantity) || (!a.Price.HasValue || a.Price.Value == price)
                    ));
            }
            var model = entities.Select(a => new ProductViewModel()
            {
                Id=a.Id,
                Status = a.Status,
                Description = a.Description,
                Category = a.CategoryId.HasValue ? a.Category.Name : null,
                Price = a.Price,
                ProductName = a.Name,
                Quantity = a.Quantity,
            });
            return DataTablesResult.Create(model, dataTablesParam);
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new ProductViewModel() { Categories = GetCategoryDictionary(), StatusDictionary = Constants.ProductStatusDictionary });
        }
        [HttpPost]
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create(ProductViewModel model)
        {
            try
            {
                model.Categories = GetCategoryDictionary();
                if (ModelState.IsValid)
                {
                    _productService.Insert(model);
                    TempData[Constants.NotifyMessage] = new NotifyModel() { Result = true, Message = string.Format("Create product {0} success",model.ProductName) };
                    return RedirectToAction("Index");
                }
                TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = string.Format("Create product {0} failed, error: {1}", model.ProductName, "Invalid data") };
            }
            catch (Exception ex)
            {
                TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = string.Format("Create product {0} failed, error: {1}", model.ProductName,ex.Message) };
            }
            model.StatusDictionary = Constants.ProductStatusDictionary;
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                TempData[Constants.NotifyMessage] = new NotifyModel() {Result = false, Message = "Product is not found"};
                return RedirectToAction("index");
            }
            var entity = _productService.FindById(id.Value);
            if (entity == null)
            {
                TempData[Constants.NotifyMessage] = new NotifyModel() { Result = false, Message = "Product is not found" };
                return RedirectToAction("index");
            }
            var imgs = entity.Images;
            var model = new ProductViewModel()
            {
                Status = entity.Status,
                Id = entity.Id,
                Categories = GetCategoryDictionary(),
                Description = entity.Description,
                Price = entity.Price,
                ProductName = entity.Name,
                Quantity = entity.Quantity,
                CategoryId = entity.CategoryId,
                StatusDictionary = Constants.ProductStatusDictionary,
                Image = imgs.Any() ? imgs.Any(a=>a.Main == true) ? imgs.First(a=>a.Main == true).Name : imgs.First().Name : null,
                ImageList = imgs.Any() ? imgs.Select(a=>a.Name).ToArray() : null
            };
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(ProductViewModel model)
        {
            
            try
            {
                var entity = _productService.FindById(model.Id);
                var imgs = entity.Images;
                ;
                model.Image = imgs.Any()
                    ? imgs.Any(a => a.Main == true) ? imgs.First(a => a.Main == true).Name : imgs.First().Name
                    : null;
                model.ImageList = imgs.Any() ? imgs.Select(a => a.Name).ToArray() : null;
                model.Categories = GetCategoryDictionary();
                model.StatusDictionary = Constants.ProductStatusDictionary;
                if (ModelState.IsValid)
                {
                    _productService.Update(model);
                    TempData[Constants.NotifyMessage] = new NotifyModel() { Message = string.Format("Update product {0} success", model.ProductName ), Result = true };
                    return RedirectToAction("Index");
                }
                TempData[Constants.NotifyMessage] = new NotifyModel() { Message = "Invalid data" ,Result = false};
            }
            catch (Exception ex)
            {
                TempData[Constants.NotifyMessage] = new NotifyModel() { Message = string.Format("Update product {0} failed. exception {1}", model.ProductName, ex.Message), Result = false };
            }
            
            return View(model);
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    _productService.Delete(id.Value);
                    return Json(new NotifyModel { Result = true, Message = "Delete success" });
                }
                catch (Exception ex)
                {
                    return Json(new NotifyModel{ Result = false, Message =  ex.Message});
                }
            }
            return Json(new NotifyModel { Result = false, Message = "Invalid data" });
        }
	}
}