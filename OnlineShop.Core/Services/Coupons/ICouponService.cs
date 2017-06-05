using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShop.Core.Models.Coupons;
using OnlineShop.Models.Entities;

namespace OnlineShop.Core.Services.Coupons
{
    public interface ICouponService : IBaseService<Coupon>
    {
        IQueryable<Coupon> GetAll();
        Coupon FindById(string id);
        void Insert(CouponViewModel model);
        void Update(CouponViewModel model);
        void Delete(string id);
    }
}
