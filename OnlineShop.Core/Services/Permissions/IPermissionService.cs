using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models.Products;
using OnlineShop.Models.Entities;

namespace OnlineShop.Core.Services.Permissions
{
    public interface IPermissionService : IBaseService<Role>
    {
        IQueryable<Role> GetAll();
        Role FindById(int id);
        void Insert(Role model);
        void Update(Role model);
        void Delete(int id);
    }
}
