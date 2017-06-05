using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using datatables.Utils.DatatableModels;
using OnlineShop.App.Securities;
using OnlineShop.Core.Models.MessageModels;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Core.Provider;
using OnlineShop.Core.Resources;
using OnlineShop.Core.Services.Orders;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.App.Areas.Administrator.Controllers
{
    public class OrdersController : BaseAdminController
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IUnitOfWork unitOfWork, IOrdersService ordersService) : base(unitOfWork)
        {
            _ordersService = ordersService;
        }
        //
        // GET: /Administrator/Orders/
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.currencySign = GetCurrencySign();
            return View();
        }
        [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult GetAll(DataTablesParam request)
        {
            var orders = _ordersService.GetAll().Select(a=> new OrdersViewModel
            {
                Id = a.Id,
                Status = a.Status,
                CreatedDate = a.CreatedDate,
                User = a.Account.Username,
                Total = a.OrderDetails.Sum(x=>x.Price * x.Quantity)
            });
            return DataTablesResult.Create(orders, request);
        }

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new Order());
        }

        //[HttpPost]
        //public ActionResult Create(Order model)
        //{
        //    _ordersService.Insert(model);
        //    return Json(new { result = true });
        //}

        [MyAuthorize(Roles = "Admin")]
        public ActionResult Detail(int? id)
        {
            ViewBag.currencySign = GetCurrencySign();
            if (!id.HasValue)
            {
                TempData[Constants.NotifyMessage] = new NotifyModel
                {
                    Result = false,
                    Message = string.Format(Resource.MsgIsNotFound, "Selected order")
                };
                return RedirectToAction("Index");
            }
            var entity = _ordersService.FindById(id.Value);
            var model = new OrderDetailViewModel()
            {
                Id = entity.Id,
                Status = entity.Status,
                Items = entity.OrderDetails != null
                    ? entity.OrderDetails.Select(a => new CartItemViewModel()
                    {
                        Price = a.Price,
                        Quantity = a.Quantity,
                        Description = a.Product.Description,
                        ProductName = a.Product.Name,
                        ProductId = a.ProductId,

                    }).ToList()
                    : null
            };
            return View(model);
        }
       
        //[HttpPost]
        //public ActionResult Delete(int? id)
        //{
        //    if (id.HasValue)
        //    {
        //        _ordersService.Delete(id.Value);
        //        return Json(new { result = true });
        //    }
        //    return Json(new { result = false });
        //}
	}
}