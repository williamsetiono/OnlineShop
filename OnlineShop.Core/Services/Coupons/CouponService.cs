using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShop.Core.Models.Coupons;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Coupons
{
    public class CouponService : BaseService<Coupon>, ICouponService
    {
        public CouponService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<Coupon> GetAll()
        {
            return Repository.GetAll();
        }

        public Coupon FindById(string id)
        {
            return Repository.Get(id);
        }

        public void Insert(CouponViewModel model)
        {
            var entity = new Coupon
            {
                Id = Guid.NewGuid().ToString(),
                Discount = model.Discount,
                Status = true,
                End_Date = model.EndDate,
                Start_Date = model.StartDate
            };
            Repository.Insert(entity);
            UnitOfWork.Save();
        }

        public void Update(CouponViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new Exception("Coupon is not found");
            }
            var entity = Repository.Get(model.Id);
            entity.Discount = model.Discount;
            entity.Status = model.Status;
            entity.End_Date = model.EndDate;
            entity.Start_Date = model.StartDate;
            Repository.Update(entity);
            UnitOfWork.Save();
        }

        public void Delete(string id)
        {
            var entity = Repository.Get(id);
            if (entity == null) throw new Exception("Coupon is not found");
            Repository.Delete(entity);
        }
    }
}
