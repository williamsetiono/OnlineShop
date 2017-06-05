using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Core.Provider;
using OnlineShop.Models.Entities;
using OnlineShop.Models.Repositories;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Orders
{
    public class OrdersService : BaseService<Order>,IOrdersService
    {
        private readonly IRepositoryBase<Product> _productRepository;
        private readonly IRepositoryBase<Account> _accountRepository;
        private readonly IRepositoryBase<Coupon> _couponRepository;
        public OrdersService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _productRepository = unitOfWork.Repository<Product>();
            _accountRepository = unitOfWork.Repository<Account>();
            _couponRepository = unitOfWork.Repository<Coupon>();
        }

        public IQueryable<Order> GetAll()
        {
            return Repository.GetAll();
        }

        public Order FindById(int id)
        {
            return Repository.Get(id);
        }

        public int Insert(CartViewModel model)
        {
            var user = _accountRepository.GetAll(a => a.Username.Equals(model.UserName)).FirstOrDefault();
            if (user == null) throw new Exception(string.Format(Resources.Resource.MsgUserNotFound, model.UserName));
            var items = model.Items.Select(a=>a.ProductId);
            var products = _productRepository.GetAll(a => items.Any(x => x == a.Id) && a.Status == (int)Constants.EnumProductStatus.Stocking).ToList();
            var now =(DateTime?) DateTime.Now;
            if (products.Any(x => items.All(a => a != x.Id)))
            {
                throw new Exception(string.Format("Product is not found"));
            }
            var coupon = !string.IsNullOrWhiteSpace(model.CouponId) 
                ? _couponRepository.GetAll(a=> a.Id == model.CouponId && a.Status && DbFunctions.TruncateTime(now) <= DbFunctions.TruncateTime(a.End_Date)).FirstOrDefault() 
                : null;
            var order = new Order()
            {
                Account = user,
                CreatedDate = DateTime.Now,
                Status = (int)Constants.EnumOrderStatus.Solve,
                CouponId = coupon != null ? coupon.Id : null,
                OrderDetails = new List<OrderDetail>()
            };
            if (coupon != null)
            {
                coupon.Status = false;
            }
            foreach (var item in model.Items)
            {
                var product = products.FirstOrDefault(a => a.Id == item.ProductId);
                order.OrderDetails.Add(new OrderDetail()
                {
                    Order = order,
                    Price = product.Price.HasValue? product.Price.Value : 0,
                    Quantity = item.Quantity,
                    Product = product
                });
            }
            Repository.Insert(order);
            UnitOfWork.Save();
            return order.Id;
        }

        public void Update(Order model)
        {
            Repository.Update(model);
            UnitOfWork.Save();
        }

        public void Delete(int id)
        {
            var entity = Repository.Get(id);
            Repository.Delete(entity);
        }
    }
}
