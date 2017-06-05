using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Core.Models.datatables;
using OnlineShop.Core.Models.Products;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Resources;
using OnlineShop.Core.Services.Products;
using OnlineShop.Core.Services.Settings;
using OnlineShop.Core.Services.Users;
using OnlineShop.Models.Entities;

namespace OnlineShop.App.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductService _productService;
        public HomeController(IProductService productService, IUserService userService, ISettingService settingService)
            : base(userService,settingService)
        {
            _productService = productService;
        }
        public ActionResult Index(int? id)
        {
            ViewBag.ShowMenu = true;
            return View();
        }

        public ActionResult GetItems(DatatableData<ProductViewModel> datatableRequest, int? categoryId)
        {
            var imgPath = Constants.ImagePath + "/";
            var entities = _productService.GetAll().Where(a=>a.Status == (int) Constants.EnumProductStatus.Stocking);
            if (categoryId.HasValue && categoryId > 0)
            {
                entities = entities.Where(a => a.CategoryId == categoryId.Value || a.Category.ParentId == categoryId);
            }
            var models = entities.Select(a => new ProductViewModel()
            {
                Id = a.Id,
                Price = a.Price,
                ProductName = a.Name,
                Image = a.Images.Any() ?
                    a.Images.Any(x => x.Main == true) ?
                         imgPath + a.Images.FirstOrDefault(x => x.Main == true).Name : imgPath + a.Images.FirstOrDefault().Name :
                      null
            }).OrderBy(a=>a.Price);
            return Json( DatatableModel.Refresh(models,datatableRequest.Size,datatableRequest.Index));
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Detail(int? id)
        {
            var imgPath = Constants.ImagePath + "/";
            if (!id.HasValue)
            {
                TempData[Constants.NotifyMessage] = string.Format(Resource.MsgIsNotFound, "Product item");
                return RedirectToAction("index");
            }
            var entity = _productService.FindById(id.Value);
            if(entity == null || entity.Status == (int)Constants.EnumProductStatus.OutOfStock)
            {
                TempData[Constants.NotifyMessage] = string.Format(Resource.MsgIsNotFound, "Product item");
                return RedirectToAction("index");
            }
            var imgProducts = entity.Images.Any() ? entity.Images.Select(a => imgPath + a.Name).ToList() : new List<string>();
            var model = new ProductDetailViewModel()
            {
                Id = entity.Id,
                Price = entity.Price.HasValue ? entity.Price.Value : 0,
                Quantity = 1,
                Description = entity.Description,
                ProductName = entity.Name,
                Status = Constants.ProductStatusDictionary[entity.Status],
                Imgs = imgProducts.Any() ? imgProducts.ToArray() : null,
                MainImg =
                    entity.Images.Any(a => a.Main == true)
                        ? imgPath + entity.Images.First(a => a.Main == true).Name
                        : imgProducts.FirstOrDefault()
            };
            return View(model);
        }
    }
}