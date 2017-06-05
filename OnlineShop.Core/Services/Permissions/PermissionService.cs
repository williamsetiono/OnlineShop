using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using OnlineShop.Core.Models.Products;
using OnlineShop.Models.Entities;
using OnlineShop.Models.UnitOfWork;

namespace OnlineShop.Core.Services.Permissions
{
    public class PermissionService : BaseService<Role>, IPermissionService
    {
        public PermissionService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IQueryable<Role> GetAll()
        {
            return Repository.GetAll();
        }

        public Role FindById(int id)
        {
            return Repository.Get(id);
        }

        public void Insert(Role model)
        {
            Repository.Insert(model);
            UnitOfWork.Save();
        }

        public void Update(Role user)
        {
            Repository.Update(user);
            UnitOfWork.Save();
        }

        public void Delete(int id)
        {
            var user = Repository.Get(id);
            Repository.Delete(user);
            UnitOfWork.Save();
        }
    }
}
