using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models.Orders;
using OnlineShop.Models.Entities;

namespace OnlineShop.Core.Services.Orders
{
    public interface IOrdersService : IBaseService<Order>
    {
        IQueryable<Order> GetAll();
        Order FindById(int id);
        int Insert(CartViewModel model);
        void Update(Order model);
        void Delete(int id);
    }
}
