using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnlineShop.Core.Models;
using OnlineShop.Models.Entities;

namespace OnlineShop.Core.Services.Categories
{
    public interface ICategoryService: IBaseService<Category>
    {
        IQueryable<Category> GetAll();
        Category FindById(int id);
        void Insert(Category entity);
        void Update(Category entity);
        void Delete(int id);
    }
}
